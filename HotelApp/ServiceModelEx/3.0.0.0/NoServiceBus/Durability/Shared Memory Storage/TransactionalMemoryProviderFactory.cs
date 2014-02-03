// © 2011 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net


using System;
using System.ServiceModel.Persistence;
using ServiceModelEx.Durability;

namespace ServiceModelEx
{
   public class TransactionalMemoryProviderFactory : MemoryProviderFactory
   {
      public override PersistenceProvider CreateProvider(Guid id)
      {
         return new TransactionalMemoryProvider(id);
      }
   }
}