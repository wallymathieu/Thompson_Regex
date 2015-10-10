using System;
using NUnit.Framework;

namespace Regexp
{
    [TestFixture]
    public class SomeTest
    {
        [Test]
        public void Test(){
            Assert.AreEqual ("abc||", ThompsonRegex.re2post ("a|b|c"));
            Assert.AreEqual ("ab.c|", ThompsonRegex.re2post ("ab|c"));
        }

        [Test]
        public void patch_a_linked_list_of_a_single_element_replaces_that_element(){
            var ptr = new Ptrlist{ next=null, s =new State { value=1 } };
            ThompsonRegex.patch (ptr, new State{ value = 2 });
            Assert.AreEqual (new Ptrlist{ next = null, s = new State { value = 2 } }, ptr);
        }

        [Test]
        public void patch_a_linked_list_of_many_elements_results_in_all_elements_being_replaced(){
            var ptr = new Ptrlist{ next=new Ptrlist{
                    next=null,
                    s=new State{ value=2 }
                }, 
                s =new State { value=1 } 
            };
            ThompsonRegex.patch (ptr, new State{ value = 3 });
            Assert.AreEqual (new Ptrlist{ next=new Ptrlist{
                    next=null,
                    s=new State{ value=3 }
                }, 
                s =new State { value=3 } 
            }, ptr);
        }
    }
}

