# Algoritmo do Banqueiro - Trabalho Prático SO

Implementação do Algoritmo do Banqueiro em C# para a disciplina de Sistemas Operacionais.

## Sobre o Projeto

O programa simula o Algoritmo do Banqueiro com múltiplas threads (clientes) competindo por recursos. O sistema garante que nenhuma alocação coloque o sistema em estado inseguro (prevenindo deadlocks).

## Notas

Este trabalho foi desenvolvido com auxílio de IA, dado o nível de complexidade do tema para o estágio atual do curso.

## Tecnologia

- C# / .NET 10

## Como executar

### Pré-requisitos

Ter o [.NET SDK](https://dotnet.microsoft.com/download) instalado.

### Rodando o programa

Clone o repositório:
```bash
git clone https://github.com/SEU_USUARIO/AlgoritmoBanqueiro_SO
cd AlgoritmoBanqueiro_SO
```

Execute passando os recursos como argumentos:
```bash
dotnet run 10 5 7
```

Onde `10 5 7` são as quantidades disponíveis de cada tipo de recurso (3 tipos no total, conforme o enunciado).

### Encerrando

Pressione **Enter** no terminal para encerrar o programa.

## Estrutura do código

- `Program.cs` — contém as classes `Banqueiro` (lógica do algoritmo) e `Program` (ponto de entrada)

## Saída esperada

```
=== Algoritmo do Banqueiro ===
Recursos iniciais: [10, 5, 7]
Clientes: 5
==============================

Todas as threads iniciadas. Pressione Enter para encerrar.

[Cliente 0] Recursos alocados com sucesso.
[Cliente 2] Pedido negado: estado inseguro.
[Cliente 1] Recursos alocados com sucesso.
[Cliente 0] Recursos liberados.
```
