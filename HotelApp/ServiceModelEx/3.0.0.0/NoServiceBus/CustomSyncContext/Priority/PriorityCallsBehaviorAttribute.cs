// © 2011 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;

namespace ServiceModelEx.CustomSyncContext.Priority
{
   [AttributeUsage(AttributeTargets.Class)]
   public class PriorityCallsBehaviorAttribute : ThreadPoolBehaviorAttribute
   {
      public PriorityCallsBehaviorAttribute(uint poolSize,Type serviceType) : this(poolSize,serviceType,null)
      {}
      public PriorityCallsBehaviorAttribute(uint poolSize,Type serviceType,string poolName) : base(poolSize,serviceType,poolName)
      {}
      protected override ThreadPoolSynchronizer ProvideSynchronizer()
      {
         if(ThreadPoolHelper.HasSynchronizer(ServiceType) == false)
         {
            return new PrioritySynchronizer(PoolSize,PoolName);
         }
         else
         {
            return ThreadPoolHelper.GetSynchronizer(ServiceType);
         }
      }
   }
}