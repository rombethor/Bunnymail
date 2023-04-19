# Bunnymail

## The purpose

The point of this application is to receive requests to send emails over **RabbitMQ** and then dispense those emails as templated messages via **SendGrid**.

## Configuration

Ensure that the configuration, or environment variables, contain the following keys:

| key | description |
| --- | ----------- |
| sendgridapikey | The API key for SendGrid |
| rabbithost | The endpoint of the RabbitMQ message broker |
| rabbituser | The username for the RabbitMQ message broker |
| rabbitpass | The password for the RabbitMQ message broker |
| database | A connection string pointing to the destination of the Sqlite configuration database file |
| username | A username for accessing the HTTP API and changing the config |
| password | A password for accessing the HTTP API and changing the config |

## Kubernetes Manifests

[Here](Bunnymail.yaml) are the Kubernetes manifests which can be used to set up Bunnymail in k8s.

## Functionality

A message similar to the following is sent via the message queue:

```json
{
	"event": "password-reset",
	"recipient": "example@contoso.com",
	"data": {
		"resetLink": "https://contoso.com/pwdreset?q=jhr3D3rTU28D734D="
	}
}
```

The `event` specified the template to use.  This maps to a SendGrid template in the Sqlite database which is turned into a message.

Event-Template mapping requires the following parameters, in addition to the `event` name:

```json
{
	"templateId": "d-mytemplateid123",
	"fromAddress": "no-reply@contoso.com",
	"fromName": "Hypercombiglobalmeganet"
}
```

