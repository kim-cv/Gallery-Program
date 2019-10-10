# Gallery-Program

I started this project becayse I want to keep myself up to date on .NET.

This solution consist of multiple projects including some unittest, in general its an WPF gallery application and a ASP.net API and both is using .NET Core 3.0.

![showcase](https://github.com/kim-cv/Gallery-Program/blob/master/showcase.gif)

WPF uses the following
- Patterns: MVVM, Repository, Dependency Injection.
- Local SQLite database.
- Lazy loading images while scrolling.
- Converting images to thumbs on separate threads for increased performance.
- Loading images from filesystem with streams including a memorystream to prevent file locking.
- User controls for reusing a UI component.
- Custom Font (Font Awesome).

API uses the following
- Patterns: Controller, Repository, Dependency Injection.
- Authentication with JWT tokens.
- Local SQLServer
- Filesystem for uploading and requesting images.
- Load configuration from appsettings.json
- JSON validation
- Image to thumb creation
