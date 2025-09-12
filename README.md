# document-intelligence-example
How to use Azure Document Intelligence


# Run this
1. Start the WebAPI project. 

```
cd FitnessExercises.WebAPI
dotnet run
```

2. Run the Vue web application project.

```
cd FitnessExercisesWebApp
npm install
npm run dev
```

3. Go to a browser and open `http://localhost:3000`


# Anti Foregery on the upload endpoint
Antiforgery has been disabled on the upload endpoint. Probably not a good idea in production.

When using the upload endpoint in a web application, you might encounter issues with anti-forgery tokens. To resolve this, you can disable the anti-forgery token validation for the specific upload endpoint. This can be done by adding the `[IgnoreAntiforgeryToken]` attribute to the action method handling the upload.

Alternatively you can use antiforgery tokens in your requests. This involves generating a token on the server side and including it in your upload requests. The server will then validate the token to ensure the request is legitimate.
