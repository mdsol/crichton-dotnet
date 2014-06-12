# Representor Builder

The ```RepresentorBuilder``` class allows you to build ```CrichtonRepresentor``` objects using a simple interface, instead of worrying about manually setting properties on the ```CrichtonRepresentor``` object. This is the preferred and recommended way of constructing ```CrichtonRepresentor``` objects.

Example usage:

```csharp

IRepresentorBuilder builder = new RepresentorBuilder();

builder.SetSelfLink("/api/objects/1"); // set the self link
builder.SetAttributesFromObject(myObject); // myObject is a model class
builder.AddTransition("parent", "/api/parent_objects/2"); // adds a transition

// create an embedded resource
IRepresentorBuilder embeddedResourceBuilder = new RepresentorBuilder();
embeddedResourceBuilder.SetAttributesFromObject(myEmbeddedResource);
embeddedResourceBuilder.SetSelfLink("/api/embedded_resources/3");

// add the embedded resource to the builder
builder.AddEmbeddedResource("child", embeddedResourceBuilder.ToRepresentor());

// get the representor object
var representor = builder.ToRepresentor();

// you can then use a serializer
var serializer = new HalSerializer();
var json = serializer.Serialize(representor);

```

You should whenever possible use the ```IRepresentorBuilder``` interface and not depend on the concrete class. Try hooking up your favorite DI framework.

## ```IRepresentorBuilder``` methods

The following methods are available on IRepresentorBuilder.

### ```CrichtonRepresentor ToRepresentor()```
Create a CrichtonRepresentor object from this builder instance. Returns a CrichtonRepresentor object.

### ```void SetSelfLink(string self)```
Sets the Self Link on the CrichtonRepresentor object that you are building

Param | Description
--- | ---
self | The Self Link Uri 

### ```void SetAttributes(JObject attributes)```
Sets the CrichtonRepresentor attributes from a JSON.NET JObject.

Param | Description
--- | ---
attributes | A JSON.NET JObject 

### ```void SetAttributesFromObject(object data)```
Sets the CrichtonRepresentor attributes from an abitrary object. This object will be internally converted into a JObject using JSON.NET, so all standard JSON.NET attributes apply.

Param | Description
--- | ---
data | An object, Model, ViewModel etc 

### ```void AddTransition(CrichtonTransition transition)```
Adds a CrichtonTransition to the current CrichtonRepresentor that you are building.

Param | Description
--- | ---
transition | A CrichtonTransition object 


### ```void AddTransition(string rel, string uri = null, string title = null, string type = null, bool uriIsTemplated = false, string depreciationUri = null, string name = null, string profileUri = null, string languageTag = null)```
Adds a transition to the current CrichtonRepresentor that you are building.

Param | Description
--- | ---
rel | The link relation. 
uri | The Uri of the transition 
title | The title of the transition 
type | The type of the transition 
uriIsTemplated | True if the Uri is a templated Uri, false if not. 
depreciationUri | If the transition has been deprecated, a link to a Uri explaining the deprecation 
name | The name of the transition. Can be used as an alternative or subcategory of title. 
profileUri | Uri to an http://alps.io/ or similar profile. 
languageTag | Language of the transition, as per RFC 5988 http://tools.ietf.org/html/rfc5988 


### ```void AddEmbeddedResource(string key, CrichtonRepresentor resource)```
Adds an embedded resource. There can be multiple resources with the same key, in which case a collection of resources will be built.

Param | Description
--- | ---
key | The embedded resource key 
resource | The embedded resource as represented by a CrichtonRepresentor 


### ```void SetCollection(IEnumerable<CrichtonRepresentor> representors;```
Sets the CrichtonRepresentor you are building to be a collection instead of a single object.

Param | Description
--- | ---
representors | An enumerable of CrichtonRepresentors 


### ```void SetCollection<T>(IEnumerable<T> collection, Func<T, string> selfLinkFunc)```
Sets the CrichtonRepresentor you are building to be a collection instead of a single object.

Example use:

```csharp

public class MyObject {
    public int Id { get; set;}
    public string Name { get; set;}
}

var myObjectCollection = new List<MyObject> { 
    new MyObject { Id = 1, Name = "Brian"}, 
    new MyObject { Id = 2, Name = "Terrance"}
};

IRepresentorBuilder builder = new RepresentorBuilder();
builder.SetCollection(myObjectCollection, o => "api/objects/" + o.Id);

```

Param | Description
--- | ---
Type T | The type of the object contained in the collection, such as a Model or ViewModel type
collection | The collection of objects that represent the collection. JSON.NET will be used to serialize the objects, so the objects can have standard JSON.NET attributes. 
selfLinkFunc | A function that defines the Self Link. This will be called on each object to populate the Self Link of the CrichtonRepresentor. 