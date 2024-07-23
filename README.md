# Angular to Blazor conversion example
## Overview
We won't have a discussion about which is better for you, Angular or Blazor?
We will only focus on how to convert an existing Angular application into a Blazor application.
What problems could we expect?
- Different UI Library. You need to select a new UI library with similar controls.
- Different page rendering and user experience. It will be very difficult to achieve that the user won't be able to see the difference between an Angular application and a Blazor application.
- Converting the main application. It could be general tips for converting components only.

## General information

### Blazor render modes

Starting with .NET version 8, blazor has 4 application rendering modes:
- Static Server. Static server-side rendering (static SSR), non interactive
- Interactive Server. Interactive server-side rendering (interactive SSR) using Blazor Server.
- Interactive WebAssembly. Client-side rendering (CSR) using Blazor WebAssembly.
- Interactive Auto.	Interactive SSR using Blazor Server initially and then CSR on subsequent visits after the Blazor bundle is downloaded.

> See More: [ASP.NET Core Blazor render modes](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/render-modes?view=aspnetcore-8.0), [Blazor hosting models (Blazor in .NET 8: Full stack Web UI )](https://chrissainty.com/blazor-in-dotnet-8-full-stack-web-ui/), [Blazor Render Modes in .NET 8](https://dvoituron.com/2024/01/23/blazor-render-modes/)

Streaming rendering is a small enhancement over SSR. It allows pages that need to execute long running requests to load quickly via an initial payload of HTML. 

> See more: [Components streaming rendering](https://learn.microsoft.com/en-us/aspnet/core/blazor/components/rendering?view=aspnetcore-8.0#streaming-rendering), [Blazor in .NET 8: Server-side and Streaming Rendering](https://chrissainty.com/blazor-in-dotnet-8-server-side-and-streaming-rendering/)
> If you are unsure, select Interactive Auto mode because Interactive WebAssembly, similar to Angular client-side rendering, has some drawbacks.

### Angular render modes
By default, Angular uses client-side rendering (CSR).
From angular version 16 it is possible to use Server Side rendering (SSR): [Angular server-side rendering](https://angular.dev/guide/ssr)

### Component structure
The component structure is the same for both languages:  

![image](Docu/pics/ComponentOverview.png)

| Part | Angular | Blazor |
|--------|---------|--------|
| User Interface |HTML template, component.html |component.razor |
| Code base       |TypeScript class, component.ts | components.razor.cs |
| Local CSS styles | CSS styles, component.css |component.razor.css |

> **Note:** It is also possible to include a code-behind section in the Razor file, which is not recommended for large components.

## Example 1 - Habit Tracker
I was looking for an Angular code example with description and source code and found this:
[Learn Angular Basics by Building a Simple App Using Angular Material](https://betterprogramming.pub/learn-angular-basics-by-building-a-simple-app-using-angular-material-9bbc19aa33cf)

[Running example with source code](https://stackblitz.com/edit/habit-tracker-basic)

![image](Docu/pics/angular_app.png)
But first we need to run Angular project locally and update used Angular version 9, to the current version 18.
You can see how quickly the Angular version has changed. In 2020 was Angular version 9, In 2024 Angular version 18. See [Angular Update Guide](https://angular.dev/update-guide/)

For the Blazor version, I decided not to use a 100% visual copy. And replace edit icons to buttons.
![image](Docu/pics/blazor_app.png)

### Conversion steps
1. Select the Blazor UI library best suited for your existing Angular application. I used Blazorise.
1. Create initial Blazor application. I used NET. 8 Wizard wirh Auto mode rendering.
1. First we need to convert single typescript files to charp. Sometimes it is possible to use free online conversion tool, for example : [Convertor ts -> C# 1](https://www.codeconvert.ai/typescript-to-csharp-converter), [Convertor ts -> C# 2](https://products.codeporting.app/convert/ai/typescript-to-csharp/)
1. Create empty Razor pages/components with similar names to existing Angular pages/components. We could use razor file, cs file and css file.
1. Copy angular .html to razor, convert .ts to c#, copy .css
1. Customize Angular component to match existing UI component. `button`--> `Button`
 - not recommended to use **for** loop conversion
 
  ```html
@for (habit of habits; track habit; let i = $index) 
  {
  
  }
```
```C#
@for (var i = 0; i < habits.Length; i++)
{
    var habit = habits[i];
    // code block
}
```
better to use **foreach** instead

  ```C#
 @foreach (var habit in Habits)
 {

 }
  ```
  
  And yes, the whole logic has to be changed, see example code.
## Example 2 - Netlify 
Login: test@google.com
[Second example](https://github.com/Ismaestro/angular-example-app) must be a bit complicated.
We can find there GraphQL Api call, authentication and localization.

[Working Angular App](https://angular-example-app.netlify.app/)  
![image](Docu/pics/example2-angular.gif)

### Conversion steps
- For this example, we will choose MS FluentUI
- For localization, we will follow [MS Recommendation](https://learn.microsoft.com/en-us/aspnet/core/blazor/globalization-localization?view=aspnetcore-8.0).
  For web assembly language setting stored in local storage and for the server in cookie via controller.
  In general, in our case it will be enough to use the cookie to store the language setting. But we leave both local settings and internal storage for learning case.
  But we keep both the cookie and local storage for learning purposes.
- 
## Blazor UI Library
- [Radzen Blazor Components - free](https://blazor.radzen.com/) A set of 80+ free and open source native Blazor UI controls.
- [MudBlazor - free](https://mudblazor.com/) Blazor Component Library based on Material design with an emphasis on ease of use. Mainly written in C# with Javascript kept to a bare minimum it empowers .NET developers to easily debug it if needed. 
- [Blazorise - limited free](https://blazorise.com/) Blazorise consists of modern UI components with customizable styling, comprehensive documentation, UI design assets, and the tooling you need to build a solid foundation for your applications. Support CSS framework independent development in C# (Fluent2, Tailwind, Bootstrap4-6, Bulma, Material)
- [MS Fluent UI - free](https://github.com/microsoft/fluentui-blazor) Microsoft Fluent UI Blazor components library. For use with ASP.NET Core Blazor applications 
- [Blazored - free](https://github.com/Blazored) A collection of components and libraries for Blazor. Local Storage, Session Storage, Modalm Toast, Fluent Validation, Typeahead.
- [MatBlazor - free](https://www.matblazor.com/) Material Design components for Blazor. last changes 2021 .NET 6.0
- [Ant Design Blazor - free](https://antblazor.com) A rich set of enterprise-class UI components based on Ant Design and Blazor
- [BlazorStrap - free](https://blazorstrap.io/) Bootstrap 4/5 Components for Blazor Framework.

- [Telerik](https://www.telerik.com/blazor-ui) Develop new Blazor apps and modernize legacy web projects in half the time with a high-performing Grid and 110+ truly native, easy-to-customize Blazor UI components. Design and productivity tools are also included. 
- [Developer Express](https://www.devexpress.com/blazor/) Blazor UI Component Library ships with a comprehensive set of native Blazor components (including a DataGrid, Pivot Grid, Scheduler, Chart, Data Editors, and Reporting).
- [Syncfusion](https://www.syncfusion.com/blazor-components) The Syncfusion Blazor component library offers 85+ responsive, lightweight components, including data visualizations like DataGrid, 50+ Charts, and Scheduler, for building modern web apps. 

YouTube Video - [8 Free and Open Source Blazor UI Libraries](https://www.youtube.com/watch?v=bsu0cCjeVaw)