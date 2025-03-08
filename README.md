# Backend

Make sure you got Microsoft Entity Framework installed:

```
dotnet tool install --global dotnet-ef
```

If you already have it installed make sure to update:

```
dotnet tool update --global dotnet-ef
```

you can check if it installed:

```
dotnet ef
```

you should see a unicorn thingy and EF

like this:

```
                     _/\__       
               ---==/    \\      
         ___  ___   |.    \|\    
        | __|| __|  |  )   \\\
        | _| | _|   \_/ |  //|\\
        |___||_|       /   \\\/\\
```


For in production we gotta authenticate for security, but temp we can use local:

https://learn.microsoft.com/en-us/aspnet/core/security/?view=aspnetcore-9.0#secure-authentication-flows