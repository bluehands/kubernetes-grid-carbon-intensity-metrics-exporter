# Kubernetes Grid Carbon Intensity metrics exporter

This is a metrics exporter useful for Prometheus in combination with [kepler](https://sustainable-computing.io/) and is is part of the [Carbon Aware Computing Project](https://carbon-aware-computing.com). Data is available for Europe - for more information see the [Github Repository](https://github.com/bluehands/Carbon-Aware-Computing).

The Exporter will download and cache the data as a json-file in the GSF Carbon Aware SDK. The file contains actual and forecast data for the given computing location, which should be the location of the nodes.

## Configuration

| Key | Default | Description |
| --- | --- | --- |
| Configuration__ComputingLocation | de | The Grid Carbon Intensity location |
| Configuration__ForecastDataEndpointTemplate | https://carbonawarecomputing.blob.core.windows.net/forecasts/{0}.json | URL template to download the data. {0} is replaced by the computing location  |

## build and push docker image

```pwsh
cd ./src
docker build -t ghcr.io/bluehands/kubernetes-grid-carbon-intensity-metrics-exporter:latest -f Exporter/Dockerfile .
docker push ghcr.io/bluehands/kubernetes-grid-carbon-intensity-metrics-exporter:latest
```

## Helm

Use Helm to deploy the exporter to k8s

```pwsh
helm install grid-carbon-intensity-metrics-exporter ./Chart  --kube-context "docker-desktop" 
```