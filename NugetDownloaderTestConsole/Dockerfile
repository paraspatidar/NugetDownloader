FROM microsoft/dotnet:2.1-sdk AS builder

#COPY NuGet.Config /root/.nuget/NuGet/NuGet.Config
COPY * /proj/
RUN cd /proj
RUN ls
RUN cd /proj && ls && ./project-build.sh


# Build env image
FROM microsoft/dotnet:2.1-runtime

WORKDIR /nuget-workdir
#install vi Package
RUN apt-get update && apt-get install -y vim
RUN mkdir /nuget-workdir/Nugetdownload
COPY --from=builder /proj/out .
EXPOSE 8888

ENTRYPOINT ["dotnet"]

CMD ["NugetDownloaderTestConsole.dll"]
