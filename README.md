# Crichton.NET [![Build Status](http://crichton-dn-ci.cloudapp.net/app/rest/builds/buildType:\(id:CrichtonDotnet_Develop\)/statusIcon)](http://crichton-dn-ci.cloudapp.net/?guest=1)
Crichton is a library to simplify generating and consuming Hypermedia API responses. It has the knowledge of Hypermedia
from the Ancients.

This is a .NET/C# port of the Ruby library at http://www.github.com/mdsol/crichton.

NOTE: THIS IS UNDER HEAVY DEV AND IS NOT READY TO BE USED YET

## Components

### Crichton.Representors

This is the core library, containing Serializers and Hypermedia Representors.

* [RepresentorBuilder][].

#### Hypermedia format support

##### HAL+JSON

The following is supported in the HAL+JSON Serializer implemented by ```HalSerializer``` as defined by http://tools.ietf.org/html/draft-kelly-json-hal.

Spec | Support
--- | ---
4. Resource Objects | ✔
4.1. Reserved Properties | ✔
4.1.1. _links | ✔
4.1.2. _embedded | ✔
5. Link Objects | ✔
5.1. href | ✔
5.2. templated | ✔
5.3. type | ✔
5.4. deprecation | ✔
5.5. name | ✔
5.6. profile | ✔
5.7. title | ✔
5.8. hreflang | ✔
7. Media Type Parameters | ✔
7.1. profile | ✔
8. Recommendations | ✔
CURIE syntax | ❌

The HAL+JSON Serializer is complete apart from CURIEs.

##### HALE+JSON

The HALE+JSON Serializer supports everything the HAL+JSON Serializer does above. HALE+JSON is defined by the spec at https://github.com/mdsol/hale/blob/master/README.md. It is implemented by ```HaleSerializer```.

Spec | Support
--- | ---
4. Link Objects | ✔
4.1. method | ✔
4.2. data | ✔
4.3. render | ✔
4.4. enctype | ✔
4.5. target | ✔
5. Data Objects | ✔
5.1. Data Properties | ✔
5.1.1. type | ✔
5.1.2. data | ✔
5.1.3. scope | ✔
5.1.4. profile | ✔
5.1.5. value | ✔
5.2 Constraint Properties | ❌
5.2.1. options | ❌
5.2.2. in | ❌
5.2.3. min | ❌
5.2.4. minlength | ❌
5.2.5. max | ❌
5.2.6. maxlength | ❌
5.2.7. pattern | ❌
5.2.8. multi | ❌
5.2.9 required | ❌
5.3 Constraint Extensions | ❌
6. Resource Objects | ❌
6.1. Reserved Properties | ❌
6.1.1 _meta | ❌
7. Reference Objects | ❌
7.1. Reserved Properties | ❌
7.1.1. _ref | ❌
7.1.1.1. String values | ❌
7.1.1.2. Link Object values | ❌

The HALE+JSON Serializer is not complete. Likely 5.2 Constraint Properties will be implemented next.

##### Other formats

We hope to support other formats such as Collection+JSON, Siren etc in the future.

## Contributing
See [CONTRIBUTING][] for details.

## Copyright
Copyright (c) 2014 Medidata Solutions Worldwide. Licensed under MIT. See [LICENSE][] for details.

## Authors

* [Ed Andersen](https://github.com/edandersen) - [@edandersen](https://twitter.com/edandersen)
* Based on the work by [Mark W. Foster](https://github.com/fosdev)

[CONTRIBUTING]: CONTRIBUTING.md
[LICENSE]: LICENSE.md
[RepresentorBuilder]: docs/representor_builder.md
