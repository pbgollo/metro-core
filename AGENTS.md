# Metro Core

API .NET do sistema **MetroIsJustABus**. A arquitetura e os comandos estão no `README.md`.

## Especificação

A especificação do projeto (enunciado v1.0, 2026-09-29) está em [`docs/spec/metro-is-just-a-bus.md`](docs/spec/metro-is-just-a-bus.md). O PDF original está na mesma pasta. Há uma cópia igual em `metro-web/docs/spec/`; ao atualizar o enunciado, atualiza as duas.

Antes de alterar comportamento de produto, fronteiras entre componentes, contratos, testes de sistema, CI/CD ou deployment, leia esse ficheiro. Não inventes requisitos que o contradigam. O sistema tem de ser distribuído: um único backend REST com toda a funcionalidade não cumpre o enunciado.
