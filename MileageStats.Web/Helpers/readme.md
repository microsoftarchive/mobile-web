# Mustache

Mustache, http://mustache.github.com/, is a templating library (somewhat like Razor) that 
has been implmented in a number of languages. It has become a popular choice for client-side
templating due to its simplicity and an implementation in JavaScript.

We decided to use Mustache for the client-side templating. In the [legacy application](http://silk.codeplex.com/),
Project Silk had two sets of templates; one for the server-side rendering using Razor and
another for the client-side rendering using the now deprecated [jQuery Templates](http://api.jquery.com/category/plugins/templates/).
 
In Project Liike, we wanted to push the idea of having one set of templates that could
be shared between both the client and the server. We decided to create a set of Razor templates
that could be rendered as Mustache templates. In order to do this, we had to create  
several helper methods that would enable us to generate a Mustache-compatibile template.

For details about the impact of this choice, both positive and negative, see [Server-side techniques for composing the views]().

## A few technical notes

`MustacheHelper<TModel>` is used to provide a hook in the Razor views. This is done in the 
`MileageStats.Web.WebViewPage<TModel>` class, which is designated as the base class for pages
in \Views\Web.config.

`MustacheTemplateExtensions` contains the bulk of the Mustache-related logic. Many of its methods simply
mirror the ones found on the default `HtmlHelper`.

The method `RenderAsMustacheTemplate` invokes the standard `RenderPartial` while overwritting the 
`ViewDataDictionary` with a single well-known value. We then use the `IsRenderingForMustache` to 
decide whether or not to render in the Mustache format or to standard markup.

Is it important to note that there will be no model when rendering for Mustache views.
This means that we will need to rely on [expressions](http://msdn.microsoft.com/en-us/library/bb397951.aspx)
and not on an instance of the model. For example, if our Razor view contained

	@Model.SomeProperty

we would receive a null reference exception when attempting to render it for Mustache.

The method `Value` is the workaround for this, as it resolves an expression into a Mustache formatted 
field. In the case of standard rendering, it uses the expression to extract the corresponding value from
the given model.

These helpers also make a few assumptions about the data that will eventually be bound to the Mustache 
templates. For example, `RouteValue` assumes that the JSON object bound to the template will include a 
property named `__route__` that contains data about the current route (see transtion.js for how that 
is attached). Likewise, `ViewBag` assumes a property named `__view__` (see ContentTypeAwareResult.cs for 
how this data is attached).

Finally, you will notice the classes `DisposableEnumerable<T>` and `DisposableEnumerator<T>` in the 
Mustache folder. These classes are used for rendering loops and section in Mustache (see the Mustache
documentation mentioned above). If you examine the `Loop` on `MustacheTemplateExtensions` you'll see
that it uses these classes. In turn, `Loop` is invoked in a Razor view like this:

	@foreach (var vehicle in Mustache.Loop(m => m.VehicleListViewModel.Vehicles))
    {
		<li>@vehicle.Name</li>
	}

The `foreach` automatically disposes of the instance of `DisposableEnumerable<T>` once all the items
have been enumerated. This allows us to emit a Mustache identifier after the loop as well as before
it. The result would look like this:

	{{#VehicleListViewModel.Vehicles}}
		<li>{{Name}}</li>
	{{/VehicleListViewModel.Vehicles}}