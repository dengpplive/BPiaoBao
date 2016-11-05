using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD
{
   public abstract class EntityBase
    {

      protected abstract string GetIdentity();

       private List<BusinessRule> _brokenRules = new List<BusinessRule>();
       protected virtual void Validate() { }
       public IEnumerable<BusinessRule> GetBrokenRules()
       {
           _brokenRules.Clear();
           Validate();
           return _brokenRules;
       }
       protected void AddBrokenRule(BusinessRule businessRule)
       {
           _brokenRules.Add(businessRule);
       }
       public override bool Equals(object entity)
       {
           return entity != null
               && entity is EntityBase
               && this == (EntityBase)entity;
       }
       public override int GetHashCode()
       {
           return this.GetIdentity().GetHashCode();
       }
       public static bool operator ==(EntityBase entity1, EntityBase entity2)
       {
           if ((object)entity1 == null && (object)entity2 == null)
               return true;
           if ((object)entity1 == null || (object)entity2 == null)
               return false;
           if (entity1.GetIdentity() == entity2.GetIdentity())
               return true;
           return false;
       }
       public static bool operator !=(EntityBase entity1, EntityBase entity2)
       {
           return (!(entity1 == entity2));
       }
    }
}
