# Omniscript.CaseStudy

## Install instrustions (for MS Windows).
1. Download and install the latest version of the "**Erlang/OTP**" from it's [downloading page](https://www.erlang.org/downloads "downloading page").
2. Download and install the latest version of the "**RabbitMQ**" from it's [downloading page](https://www.rabbitmq.com/install-windows.html "downloading page").
3. Disconnect from any VPN services.
4. Run following script from the command line with administrative privileges:
```bash
ipconfig /release & ipconfig /renew
```
5. Run Windows Powershell with administrative privileges, and run one of the following script lines.

- If you want to use debug developer exception pages and Swagger GUI:

```shell
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development", "Machine")
```

- If you want to use just Swagger GUI:

```shell
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Staging", "Machine")
```

- If you want to use bare apps with no GUI, to access it through apps, like "Postman" (not recomended, due to code documentation is in Swagger GUI): 

```shell
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", "Machine")
```

## Run instrustions (for MS Windows).
1. Go to the project's "\Omniscript.CaseStudy\bin" folder, than run "**Server**" shortcut first. Alternatively run "\Omniscript.CaseStudy\bin\Server\Omniscript.CaseStudy.Server.exe" executable.
2. Go to the project's "\Omniscript.CaseStudy\bin" folder, than run "**Client**" shortcut first. Alternatively run "\Omniscript.CaseStudy\bin\Client\Omniscript.CaseStudy.Client.exe" executable.
3. Follow the documentation from the project's Swagger.