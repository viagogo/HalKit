### New in 0.5.0 (Released 2015/07/07)
* Started using ResourceContractResolver for serialization for an ~2x performance improvement

### New in 0.4.0 (Released 2015/07/02)
* Improved ResourceConverter performance

### New in 0.3.0 (Released 2015/06/11)
* Added RelAttribute that can be used to adorn Link properties
* Updated ResourceConverter to also support serializing Resources

### New in 0.2.1 (Released 2015/04/08)
* Fixed a bug in HttpConnection where we weren't defaulting request body Content-Type to application/hal+json

### New in 0.2.0 (Released 2015/04/07)
* Added IRequestParameters DTO that represents the query params and headers to be used in a request
* Re-structured the namespaces of the model objects
* Made IJsonSerializer interface synchronous

### New in 0.1.0 (Released 2015/03/30)
* Initial release