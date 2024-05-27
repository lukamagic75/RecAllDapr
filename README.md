## Language

- [English](#english)
- [中文](#中文)

---

### English

# RecAllDapr Microservices Error Workbook Project

> **Note**: This project is based on the microservices architecture project by Professor Zhang Yin from Northeastern University, adapted from the original .NET 8.0 version. Original project link: [https://gitee.com/zhangyin-gitee/rec-all-dapr-2](https://gitee.com/zhangyin-gitee/rec-all-dapr-2)

## Introduction

The `RecAllDapr` project is primarily implemented in C# using the [Dapr](https://dapr.io/) microservices framework, aiming to provide a highly scalable and maintainable service architecture. The project involves various technical practices, including Domain-Driven Design (DDD), event-driven architecture, and Token-based authentication. This project chooses the simple theme of an error workbook to familiarize with the entire microservices architecture and learn related technologies.

## Technology Stack & Features

- **.NET Core 8.0**: The project is developed using .NET 8.0, ensuring all NuGet packages are compatible with this version.
- **EF Core**: Automates database generation and management.
- **Dapr**: Simplifies state management, service calls, etc., in microservices architecture.
- **Domain-Driven Design (DDD)**: Emphasizes implementing business logic through domain models.
- **Event Bus (RabbitMQ)**: Handles asynchronous message communication, enhancing service decoupling.
- **Authentication & Token Penetration**: Uses Token penetration mechanism for secure communication between microservices.
- **Docker**: Container technology used for deploying and managing application containers, supporting rapid deployment and scalability of the project.

## Functional Features

- **Domain-Driven Design**: Clearly defines service boundaries and business responsibilities.
- **Event-Driven Architecture**: Enhances the overall responsiveness and scalability of the system.
- **Microservices Governance**: Uses various microservices components provided by Dapr, including state management, service invocation, and message publishing/subscribing.

---

### 中文

# RecAllDapr 基于微服务架构使用C#实现的错题本

> **注意**: 本项目基于东北大学张引副教授的微服务架构项目 .NET 8.0 版本项目的改编版本，原项目链接：[https://gitee.com/zhangyin-gitee/rec-all-dapr-2](https://gitee.com/zhangyin-gitee/rec-all-dapr-2)

## 简介

`RecAllDapr` 项目绝大部分由C#实现，采用 [Dapr](https://dapr.io/) 微服务框架，旨在提供一个高度可扩展和易于维护的服务架构。项目涉及多种技术实践，包括领域驱动设计（DDD）、事件驱动架构以及Token穿透的身份认证。本项目选取了错题本这个简单的主题，用于熟悉整个微服务架构的搭建，并学习相关技术。

## 技术栈与特点

- **.NET Core 8.0**：项目使用 .NET 8.0 版本开发，确保所有 NuGet 包与此版本兼容。
- **EF Core**：自动化数据库生成和管理。
- **Dapr**：简化微服务架构中的状态管理、服务调用等。
- **领域驱动设计 (DDD)**：强调通过领域模型来实现业务逻辑。
- **事件总线 (RabbitMQ)**：处理异步消息通信，增强服务解耦。
- **身份验证与 Token 穿透**：使用 Token 穿透机制来实现微服务之间的安全通信。
- **Docker**：容器化技术，用于部署和管理应用容器，支持项目的快速部署和可扩展性。

## 功能特点

- **领域驱动设计**：清晰定义服务边界和业务职责。
- **事件驱动架构**：提高系统整体的响应性和可扩展性。
- **微服务治理**：采用 Dapr 提供的各种微服务组件，包括状态管理、服务调用和消息发布/订阅。
