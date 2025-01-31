# SIPS Connect Platform Documentation

## Table of Contents

- [SIPS Connect Platform Documentation](#sips-connect-platform-documentation)
  - [Table of Contents](#table-of-contents)
  - [Introduction](#introduction)
  - [Features](#features)
  - [Architecture Overview](#architecture-overview)
  - [API Reference](#api-reference)
    - [Transaction Gateway](#transaction-gateway)
      - [POST `/api/v1/gateway/Verify`](#post-apiv1gatewayverify)
      - [POST `/api/v1/gateway/Payment`](#post-apiv1gatewaypayment)
    - [Incoming Messages](#incoming-messages)
      - [POST `/api/v1/incoming`](#post-apiv1incoming)
      - [POST `/api/v1/incoming`](#post-apiv1incoming-1)
  - [Authentication \& Authorization](#authentication--authorization)
    - [Bearer Token Authentication](#bearer-token-authentication)
  - [Error Handling](#error-handling)
  - [Security](#security)
  - [Database Management](#database-management)
  - [Integration with SPS Public Key Repository](#integration-with-sps-public-key-repository)
  - [Contact \& Support](#contact--support)

---

## Introduction

The **SIPS Connect Platform** is a robust solution designed to facilitate the seamless sending and receiving of money between the SIPS SVIP system and local banking systems. It achieves this by translating ISO 20022 messages to and from Local Bank JSON APIs, ensuring secure and efficient financial transactions.

## Features

- **Message Translation:** Converts ISO 20022 messages from SIPS to Local Bank JSON API and vice versa.
- **Transaction Signing:** Secures transactions using private key encryption.
- **Transaction Verification:** Validates transactions using public keys integrated with the SPS Public Key Repository.
- **Data Persistence:** Stores all verifications and transactions in a secure database for audit and reference purposes.

## Architecture Overview

The SIPS Connect Platform acts as an intermediary gateway, handling both outbound and inbound financial transactions. It ensures data integrity and security through cryptographic signing and verification mechanisms while maintaining comprehensive records in its database.

## API Reference

### Transaction Gateway

The Transaction Gateway facilitates the verification and payment processes between the SIPS SVIP and local banking systems.

#### POST `/api/v1/gateway/Verify`

**Description:**  
Verifies transaction requests by processing `VerificationRequest` JSON objects from the provided adapter.

**Request:**

- **Headers:**

  - `Authorization: Bearer {api_key}:{api_secret}`
  - `Content-Type: application/json`

- **Body:**
  ```json
  {
    "VerificationRequest": {
      /* Verification details */
    }
  }
  ```

**Response:**

- **200 OK:**

  ```json
  {
    "VerificationResponse": {
      /* Verification results */
    }
  }
  ```

- **Error Responses:**
  - `400 Bad Request` – Invalid request format or parameters.
  - `401 Unauthorized` – Missing or invalid authentication credentials.
  - `404 Not Found` – Endpoint or resource not found.
  - `500 Internal Server Error` – Server encountered an unexpected condition.

#### POST `/api/v1/gateway/Payment`

**Description:**  
Processes payment requests by handling `PaymentRequest` JSON objects from the provided adapter.

**Request:**

- **Headers:**

  - `Authorization: Bearer {api_key}:{api_secret}`
  - `Content-Type: application/json`

- **Body:**
  ```json
  {
    "PaymentRequest": {
      /* Payment details */
    }
  }
  ```

**Response:**

- **200 OK:**

  ```json
  {
    "PaymentResponse": {
      /* Payment confirmation */
    }
  }
  ```

- **Error Responses:**
  - `400 Bad Request` – Invalid request format or parameters.
  - `401 Unauthorized` – Missing or invalid authentication credentials.
  - `404 Not Found` – Endpoint or resource not found.
  - `500 Internal Server Error` – Server encountered an unexpected condition.

### Incoming Messages

Processes incoming verification and payment messages from the SIPS SVIP system and triggers your callback URLs using the designated JSON adapter properties.

#### POST `/api/v1/incoming`

**Description:**  
processes Received ISO 20022 To `CB_VerificationRequest` JSON objects and forwards them to your API via the designated callback links.
**Request:**

- **Headers:**

  - `Url: {verification_callback_url}` from the Configuration
  - `Authorization: Bearer {api_key}:{api_secret}`
  - `Content-Type: application/json`

- **Body:**
  ```json
  {
    "CB_VerificationRequest": {
      /* Verification details */
    }
  }
  ```

**Response:**

- **200 OK:**

  ```json
  {
    "CB_VerificationResponse": {
      /* Verification results */
    }
  }
  ```

- **Error Responses:**
  - `400 Bad Request` – Invalid request format or parameters.
  - `401 Unauthorized` – Missing or invalid authentication credentials.
  - `404 Not Found` – Endpoint or resource not found.
  - `500 Internal Server Error` – Server encountered an unexpected condition.

#### POST `/api/v1/incoming`

**Description:**  
Processes received ISO 20022 messages into `CB_PaymentRequest` JSON objects and forwards them to your API via the designated callback links.

**Request:**

- **Headers:**

  - `Url: {verification_callback_url}` from the Configuration
  - `Authorization: Bearer {api_key}:{api_secret}`
  - `Content-Type: application/json`

- **Body:**
  ```json
  {
    "CB_PaymentRequest": {
      /* Payment details */
    }
  }
  ```

**Response:**

- **200 OK:**

  ```json
  {
    "CB_PaymentResponse": {
      /* Payment confirmation */
    }
  }
  ```

- **Error Responses:**
  - `400 Bad Request` – Invalid request format or parameters.
  - `401 Unauthorized` – Missing or invalid authentication credentials.
  - `404 Not Found` – Endpoint or resource not found.
  - `500 Internal Server Error` – Server encountered an unexpected condition.

## Authentication & Authorization

### Bearer Token Authentication

All API endpoints require authentication via the `Authorization` header using a Bearer token. The token is constructed from an `api_key` and `api_secret`.

**Token Format:**

```
Bearer api_key:api_secret
```

**Configuration:**

- **Gateway Bearer Token:**

  - `api_key`: `api_key`
  - `api_secret`: `very_strong_secret`
  - **Sample Token:** `Bearer api_key:very_strong_secret`

- **Incoming Gateway Bearer Token:**
  - `api_key`: `api_key`
  - `api_secret`: `very_strong_secret`
  - **Sample Token:** `Bearer api_key:very_strong_secret`

**Token Provisioning:**

- The Bearer token is retrieved from a configuration file or environment variable to ensure secure management of credentials.

## Error Handling

The platform uses standard HTTP status codes to indicate the success or failure of API requests:

- **400 Bad Request:** The request was invalid or cannot be served. This could be due to malformed request syntax or invalid request parameters.
- **401 Unauthorized:** Authentication credentials were missing or invalid.
- **404 Not Found:** The requested resource could not be found.
- **500 Internal Server Error:** An unexpected error occurred on the server side.

Each error response includes a relevant message detailing the nature of the error to aid in troubleshooting.

## Security

- **Transaction Signing:** All transactions are signed using a private key to ensure authenticity and integrity.
- **Transaction Verification:** Signed transactions are verified using a public key retrieved from the SPS Public Key Repository.
- **Data Encryption:** Sensitive data is encrypted both in transit and at rest to protect against unauthorized access.
- **Secure Storage:** Verification records and transaction details are securely stored in the database with appropriate access controls.

## Database Management

All verifications and transactions processed by the SIPS Connect Platform are securely stored in a PostgreSQL database, ensuring a reliable record for auditing, reconciliation, and historical reference. The database schema is designed for high-performance handling of large transactional volumes while maintaining data integrity and security. It also features automatic schema and database generation capabilities for seamless scalability.

## Integration with SPS Public Key Repository

The platform seamlessly integrates with the SPS Public Key Repository to retrieve public keys required for verifying signed transactions. This ensures that only authenticated and authorized transactions are processed, upholding the highest standards of security and trust. Configuration for this module can be managed through system configuration files or environment variables.

## Contact & Support

For further assistance, support, or inquiries related to the SIPS Connect Platform, please contact our support team:

- **Support Portal:** [www.support.sps.so](https://support.sps.so)

---

_This documentation is continuously updated and revised. For the latest information, please refer to the official SIPS Connect Platform documentation portal. Upcoming upgrades, along with a changelog and feature adaptability details, will be shared in this repository soon._
