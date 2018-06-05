# Obsoleted & Deprecated
I have moved the library to .net standard, and in doing so, I completely re-wrote the library. You can find the new library [Here](https://github.com/calico-crusade/palringo-api-std). I will not be supporting / updating / modifying this repo. I will still accept pull requests if people want me too.

# PalringoApi
A useful library made for connection to the Palringo Instant messaging service.
This library is not official, endorsed, or affliated with Palringo.

# How do I use it?
It is simple enough to use. First you install it. There are 3 separate libraries to use.
Using Nuget you can install all 3 packages or just one of them. Whatever suits your fancy!

```
PM> Install-Package PalringoApi
```
This is the primary package, the meat and potatoes.
It contains:
* The connection and packet handling to Palringo
* The subprofiling and user information parsing
* Message handling (receiving and sending)

```
PM> Install-Package PalringoApi.Plugins
```
This is an optional package, but a must have!
It contains:
* Different plugin types for interacting with Palringo
* Custom message handling and such

```
PM> Install-Package PalringoApi.Plugins.BackwardsCompatible
```
This is an optional package, and should not be used if it can be avoided.
This is just for backwards compatiblity with an old un-released version a few of you have!

# Documentation
You can find better documentation and usage of the library [Here](http://docs.dontpanicitscool.org/palringo) (Not complete)

