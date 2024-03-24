# kubernetes-grid-carbon-intensity-metrics-exporter

## build and push docker image

```pwsh
cd ./src
docker build -t sustainabilitylab.azurecr.io/kubernetes-grid-carbon-intensity-configmap-exporter:latest -f Exporter/Dockerfile .
docker push sustainabilitylab.azurecr.io/kubernetes-grid-carbon-intensity-configmap-exporter:latest
```
