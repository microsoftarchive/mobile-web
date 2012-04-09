## Reagarding the Domain

For the most part, this code has simply been carried forward fomr [Project Silk](http://silk.codeplex.com/).

The only significantly new part is `BingMapService` and the corresponding `MockMapService`. 
These classes are meant to provide a façade for accesing the actual Bing services.
Refer to the `MileageStats.Web.Controllers.GeoLocationController` to see how this service is consumed.

 ### Contracts

 These are various interfaces that are used throughout the application. 
 Many of these interfaces are from the legacy application.
 
 Of particular interest though are:

 * ICreateFillupEntryCommand
 * ICreateReminderCommand
 * ICreateVehicleCommand

 These three interfaces allow the domain to define the data necessary for creating fillups, reminders, and
 vehicles respectively. In turn, these interfaces are implemented as _form models_. By form model, we mean
 a view model that is meant to represent incoming data for a controller action. See Project Silk's 
 [Chapter 11: Server-Side Implementation](http://msdn.microsoft.com/en-us/library/hh404093.aspx) for more 
 information on this topic.

 ### Handlers

 These classes represent the core behavior of the application. They are composed and coordniated by the 
 controllers.
 They are not the primary focus of this guidance and are not necessarily representative of best practices.

 ### Models

 If the handler classes are the verbs, the the model classes are the nouns. These are the core data 
 structures used in the application. Since many of these were carried forward from the legacy application,
 they bare the vestiges of using the Entity Framework. Likewise, there are a few notable perculiariaties 
 such as the various reminder models. These are holdovers from the legacy application originally having
 three sets of models:
 
 * _entity models_ that merely held the data loaded by the O/RM
 * _domain models_ that embodied the behavior associated with the data
 * _view models_ that contained presentation logic and sometimes additional validation

  Overall, the models are not the primary focus of this guidance and should not be considered a demonstration
  of best practices.