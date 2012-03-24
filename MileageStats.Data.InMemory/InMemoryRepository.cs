/*  
Copyright Microsoft Corporation

Licensed under the Apache License, Version 2.0 (the "License"); you may not
use this file except in compliance with the License. You may obtain a copy of
the License at 

http://www.apache.org/licenses/LICENSE-2.0 

THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED 
ARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE, 
MERCHANTABLITY OR NON-INFRINGEMENT. 

See the Apache 2 License for the specific language governing permissions and
limitations under the License. */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MileageStats.Domain.Contracts;

namespace MileageStats.Data.InMemory
{
    public abstract class InMemoryRepository<T> where T : IHasIdentity
    {
        private static readonly IDictionary<int, T> _store = new ConcurrentDictionary<int, T>();

        protected IDictionary<int, T> Store
        {
            get { return _store; }
        }

        public IEnumerable<T> Set
        {
            get { return _store.Values;  }
        }

        private static int _lastId;
         
        public static int GetNextId()
        {
            return ++_lastId;
        }

        public virtual void Create(T item)
        {
            item.Id = GetNextId();
            Store.Add(item.Id, item);
        }

        public virtual T Get(int id)
        {
            if(!Store.ContainsKey(id)) throw new InvalidOperationException(string.Format("Unable to retrieve #{0} of type {1}; item was not found in store.", id, typeof(T).Name));
            return Store[id];
        }

        public virtual void Delete(int id)
        {
            if (!Store.ContainsKey(id)) throw new InvalidOperationException(string.Format("Unable to delete #{0} of type {1}; item was not found in store.", id, typeof(T).Name));
            Store.Remove(id);
        }

        public virtual void Update(T item)
        {
            if(Store.ContainsKey(item.Id))
            {
                Store[item.Id] = item;
            } else
            {
                throw new Exception(string.Format("Unable to update #{0} of type {1}; item was not found in store.", item.Id, typeof(T).Name));
            }
        }
    }
}