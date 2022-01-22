# NameValueCollection
Represents a generic implementation of __NameValueCollection__ - a generic collection of associated string keys and given type values that can be accessed either with the key or with the index.  
It is a collection that is similar to a __Dictionary__ but __NameValueCollection__ can have duplicate keys while __Dictionary__ cannot. Elements can be obtained both by index and by key.  
What makes this collection special, is that one key can contain several elements. 


```csharp
NameValueCollection<int> collection = new NameValueCollection<int>();
collection.Add("a", 123);
collection.Add("a", 456); // 123 and 456 will be inserted into the same key. 
collection.Add("b", 789); // 789 will be inserted into another key.
```
