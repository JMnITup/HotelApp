// © 2011 IDesign Inc. All rights reserved 
//Questions? Comments? go to 
//http://www.idesign.net

using System;
using ServiceModelEx.Durability;

namespace ServiceModelEx
{
   public class TransactionalMemoryProvider : MemoryProvider
   {
      public TransactionalMemoryProvider(Guid id) : base(id,new TransactionalMemoryStore<Guid,object>())
      {}
   }
}
