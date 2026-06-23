#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY /app/publish .
ADD GEANT_TLS_RSA_1.crt /usr/local/share/ca-certificates
ADD HARICA_TLS_RSA_Root_CA_2021.crt /usr/local/share/ca-certificates
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "AnonymizationService.HttpApi.Host.dll"]