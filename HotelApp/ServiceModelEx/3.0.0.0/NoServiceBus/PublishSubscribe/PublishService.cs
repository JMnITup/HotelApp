// © 2011 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using System.Diagnostics;
using System.Reflection;
using System.ServiceModel;
using System.Threading;

namespace ServiceModelEx
{
   public abstract class PublishService<T> where T : class
   {
       /* Start BPE added - James Meyer 2013-11-15 */

       //public static Func<ILogger> GetLoggerInstance { get; set; }
       public static Func<string, T[]> GetPersistentListFunction { get; set; }
       public static Func<string, T[]> GetTransientListFunction { get; set; }

       public PublishService() {
           //GetLoggerInstance = () => new Logger();
           GetPersistentListFunction = SubscriptionManager<T>.GetPersistentList;
           GetTransientListFunction = SubscriptionManager<T>.GetTransientList;
       }
       /* END BPE added - James Meyer 2013-11-15 */

       /* START BPE added - James Meyer 2013-11-15 */
       // Methods using FireEvent (usually service contract implementation methods) should be decorated with the following
       // attribute to allow for unit testing of publishers
       //[MethodImpl(MethodImplOptions.NoInlining)]
       /* END BPE added - James Meyer 2013-11-15 */
       protected static void FireEvent(params object[] args)
       {
          string action;
          string[] slashes;
          string methodName;
         /* Start BPE added - James Meyer 2013-11-15 */
         if (OperationContext.Current == null) {
             methodName = new StackFrame(1, true).GetMethod().Name;
         } else {
             /* End BPE added - James Meyer 2013-11-15 */
             action = OperationContext.Current.IncomingMessageHeaders.Action;
             slashes = action.Split('/');
             methodName = slashes[slashes.Length - 1];
         }

         FireEvent(methodName,args);
      }
      static void FireEvent(string methodName,object[] args)
      {
         PublishPersistent(methodName,args);
         PublishTransient(methodName,args);
      }
      static void PublishPersistent(string methodName,object[] args)
      {
          /* Start BPE changed - James Meyer 2013-11-15 */
         //T[] subscribers = SubscriptionManager<T>.GetPersistentList(methodName);
          T[] subscribers = GetPersistentListFunction(methodName);
         /* End BPE added - James Meyer 2013-11-15 */
         Publish(subscribers,true,methodName,args);
      }
      static void PublishTransient(string methodName,object[] args)
      {
          /* Start BPE changed - James Meyer 2013-11-15 */
          //T[] subscribers = SubscriptionManager<T>.GetTransientList(methodName);
          T[] subscribers = GetTransientListFunction(methodName);
          /* End BPE added - James Meyer 2013-11-15 */
         Publish(subscribers,false,methodName,args);
      }
      static void Publish(T[] subscribers,bool closeSubscribers,string methodName,object[] args)
      {
         WaitCallback fire = (subscriber)=>
                             {
                                Invoke(subscriber as T,methodName,args);
                                if(closeSubscribers)
                                {
                                   try
                                   {
                                      using(subscriber as IDisposable)
                                      {
                                      }
                                   }
                                   catch
                                   {}
                                }
                             };
         Action<T> queueUp = (subscriber)=>
                             {
                                ThreadPool.QueueUserWorkItem(fire,subscriber);
                             };
         subscribers.ForEach(queueUp);
      }
      static void Invoke(T subscriber,string methodName,object[] args)
      {
         Debug.Assert(subscriber != null);
         Type type = typeof(T);
         MethodInfo methodInfo = type.GetMethod(methodName);
         try
         {
            methodInfo.Invoke(subscriber,args);
         }
         catch(Exception e)
         {
            Trace.WriteLine(e.Message);
             /* Start BPE added - James Meyer 2013-11-15 */
             //if (GetLoggerInstance != null) {
             //    GetLoggerInstance.Invoke().LogException(e);
             //}
             /* End BPE added - James Meyer 2013-11-15 */
         }
      }
   }
}