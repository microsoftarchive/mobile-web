# Pagination

Many of these controllers have actions that return lists. It is generally best to
always return a paginated set of items rather than the whole list. Returning the 
entire set of data is an anti-pattern often referred to as an Unbounded Result Set.

The primary problem with unbounded result sets is that you don't know how large the
result set is going to be. You might accidentally send a lot more data to the client 
browser than you anticipated (and on a bandwidth-constrained device that has 
significant impact on the user experience).  Similarly, making unbound queries
against your database can use more resources than anticipated causing blocks.s

It is recommend to use pagination by default, and only to return unbounded result
sets when there is an explicit reason.

For information on how to implement pagination using ASP.NET MVC and the Entity Framework, 
see Julie Lerman's [Server-Side Paging with the Entity Framework and ASP.NET MVC 3](http://msdn.microsoft.com/en-us/magazine/gg650669.aspx)