# Documento de Requisitos — Sistema de Gestão de Biblioteca Universitária

- **Aluno:** Uanderson Bueno
- **Disciplina:** 26E4_5 — Projeto de Bloco
- **Entrega:** TP1
- **Versão:** 1.0 — 17/11/2026

## 1. Introdução

### 1.1 Objetivo do sistema

O sistema gerencia o acervo de uma biblioteca universitária (livros e seus exemplares físicos), os membros da comunidade acadêmica e a circulação dos exemplares: consulta, empréstimo, devolução, reserva e cobrança de multas por atraso. O objetivo é dar controle sobre o acervo e garantir que as regras de uso da biblioteca sejam aplicadas automaticamente, sem depender da memória do atendente.

### 1.2 Atores

| Ator | Descrição | O que pode fazer |
|------|-----------|------------------|
| **Membro** | Aluno, professor ou funcionário com cadastro na biblioteca. | Consultar o acervo, verificar disponibilidade, solicitar empréstimos e devoluções, realizar reservas e consultar suas próprias multas. |
| **Bibliotecário/Administrador** | Funcionário responsável pela biblioteca. | Cadastrar, consultar, atualizar e remover livros, exemplares e membros; processar empréstimos e devoluções; gerenciar reservas; registrar e controlar multas. |

### 1.3 Entidades principais

| Entidade | Atributos |
|----------|-----------|
| Livro | título, autor, ISBN |
| Exemplar | código de patrimônio, estado de conservação, livro ao qual pertence |
| Membro | nome, matrícula, tipo (aluno, professor ou funcionário) |
| Empréstimo | exemplar, membro, data de retirada, prazo de devolução, data efetiva de devolução |
| Reserva | livro, membro, data da reserva |
| Multa | membro, empréstimo relacionado, valor, situação de pagamento |

## 2. Regras de Negócio

| ID | Regra |
|----|-------|
| RN01 | O prazo padrão de cada empréstimo é de 14 dias corridos a partir da data de retirada. |
| RN02 | A multa por atraso é calculada por dia corrido após o prazo de devolução, considerando apenas os dias de atraso até a devolução efetiva do exemplar. |
| RN03 | Cada membro pode ter, no máximo, 3 empréstimos simultâneos ativos. |
| RN04 | Um exemplar somente pode ser emprestado se estiver disponível e em condições adequadas de uso. |
| RN05 | Um exemplar não pode possuir mais de um empréstimo ativo simultaneamente. |
| RN06 | A reserva somente pode ser realizada para um livro que possua exemplares atualmente emprestados; não é permitida quando todos os exemplares estiverem disponíveis. |
| RN07 | A reserva é vinculada ao livro, e não a um exemplar específico. |
| RN08 | A devolução encerra o empréstimo ativo correspondente e registra a data efetiva de devolução. |
| RN09 | Devolução após o prazo gera multa proporcional aos dias corridos de atraso, vinculada ao membro responsável pelo empréstimo. |
| RN10 | Exemplares danificados ou indisponíveis não podem ser emprestados até que sua condição seja regularizada pelo bibliotecário. |
| RN11 | Somente o Bibliotecário/Administrador pode cadastrar ou alterar registros do acervo e dos membros, processar operações administrativas e gerenciar multas. |
| RN12 | Os membros devem possuir cadastro válido para realizar empréstimos e reservas. |

## 3. Requisitos Funcionais

Prioridade: **E** = Essencial, **I** = Importante, **D** = Desejável.

### 3.1 Gestão do acervo

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF01 | O sistema deve permitir cadastrar um livro informando título, autor e ISBN, rejeitando ISBN já cadastrado. | Bibliotecário | E | Entidades, RN11 |
| RF02 | O sistema deve permitir atualizar os dados de um livro cadastrado. | Bibliotecário | I | RN11 |
| RF03 | O sistema deve permitir remover um livro, desde que nenhum de seus exemplares possua empréstimo ativo. | Bibliotecário | I | RN11 |
| RF04 | O sistema deve permitir cadastrar um exemplar informando código de patrimônio único, estado de conservação e o livro ao qual pertence. | Bibliotecário | E | Entidades, RN07 |
| RF05 | O sistema deve permitir alterar o estado de conservação de um exemplar (bom, danificado, indisponível), inclusive para regularizar um exemplar danificado. | Bibliotecário | E | RN10 |
| RF06 | O sistema deve permitir remover um exemplar, desde que ele não possua empréstimo ativo. | Bibliotecário | D | RN11 |

### 3.2 Gestão de membros

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF07 | O sistema deve permitir cadastrar um membro informando nome, matrícula única e tipo (aluno, professor ou funcionário). | Bibliotecário | E | Entidades, RN11 |
| RF08 | O sistema deve permitir consultar membros por nome ou matrícula. | Bibliotecário | I | RN11 |
| RF09 | O sistema deve permitir atualizar os dados de um membro, incluindo a situação do cadastro (ativo ou inativo). | Bibliotecário | E | RN11, RN12 |
| RF10 | O sistema deve permitir remover um membro, desde que ele não possua empréstimos ativos nem multas pendentes. | Bibliotecário | D | RN11 |

### 3.3 Consulta

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF11 | O sistema deve permitir consultar o acervo por título, autor ou ISBN. | Membro, Bibliotecário | E | Atores |
| RF12 | O sistema deve informar, para cada livro, a quantidade de exemplares disponíveis, emprestados e indisponíveis. | Membro, Bibliotecário | E | Atores, RN04 |
| RF13 | O sistema deve permitir que o membro consulte seus empréstimos ativos, com o prazo de devolução de cada um. | Membro | I | Atores |

### 3.4 Empréstimo

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF14 | O sistema deve permitir que o membro solicite o empréstimo de um exemplar. | Membro | E | Atores |
| RF15 | O sistema deve registrar o empréstimo com exemplar, membro, data de retirada e prazo de devolução calculado como data de retirada + 14 dias corridos. | Bibliotecário | E | RN01 |
| RF16 | O sistema deve recusar o empréstimo quando o membro não possuir cadastro válido. | Sistema | E | RN12 |
| RF17 | O sistema deve recusar o empréstimo quando o membro já possuir 3 empréstimos ativos. | Sistema | E | RN03 |
| RF18 | O sistema deve recusar o empréstimo quando o exemplar já possuir empréstimo ativo ou estiver danificado ou indisponível. | Sistema | E | RN04, RN05, RN10 |
| RF19 | O sistema deve recusar o empréstimo quando o membro possuir multa pendente de pagamento. | Sistema | I | Suposição S03 |

### 3.5 Devolução e multa

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF20 | O sistema deve registrar a devolução de um exemplar, encerrando o empréstimo ativo e gravando a data efetiva de devolução. | Membro (solicita), Bibliotecário (processa) | E | RN08 |
| RF21 | Na devolução, o sistema deve calcular os dias de atraso (data efetiva − prazo de devolução, quando positivo) e gerar uma multa de valor = dias de atraso × valor diário, vinculada ao membro e ao empréstimo. | Sistema | E | RN02, RN09 |
| RF22 | Após a devolução, o exemplar deve voltar a ficar disponível, exceto se o bibliotecário registrar que foi devolvido danificado. | Bibliotecário | I | RN04, RN10 |
| RF23 | O sistema deve permitir consultar multas por membro, mostrando valor e situação (pendente ou paga). | Membro (as próprias), Bibliotecário (todas) | E | RN09, RN11 |
| RF24 | O sistema deve permitir registrar o pagamento de uma multa, alterando sua situação para paga. | Bibliotecário | E | RN11 |

### 3.6 Reserva

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF25 | O sistema deve permitir que um membro com cadastro válido reserve um livro (não um exemplar), registrando livro, membro e data da reserva. | Membro | E | RN07, RN12 |
| RF26 | O sistema deve recusar a reserva quando o livro não possuir nenhum exemplar emprestado no momento. | Sistema | E | RN06 |
| RF27 | O sistema deve impedir que o mesmo membro tenha duas reservas ativas para o mesmo livro. | Sistema | I | Suposição S05 |
| RF28 | O sistema deve permitir que o bibliotecário consulte e cancele reservas, listadas por ordem de data. | Bibliotecário | I | Atores |
| RF29 | Ao registrar a devolução de um exemplar, o sistema deve avisar se existe reserva ativa para o livro, indicando o membro com a reserva mais antiga. | Sistema | D | Suposição S06 |

### 3.7 Controle de acesso

| ID | Descrição | Ator | Prior. | Origem |
|----|-----------|------|--------|--------|
| RF30 | O sistema deve restringir ao Bibliotecário/Administrador as operações de cadastro, alteração e remoção de acervo e membros, o processamento de empréstimos e devoluções e a gestão de multas. | Sistema | E | RN11 |

## 4. Requisitos Não Funcionais

| ID | Categoria | Descrição | Verificação |
|----|-----------|-----------|-------------|
| RNF01 | Restrição técnica | O sistema deve ser desenvolvido em C# sobre o .NET 10 (LTS). | Inspeção do arquivo `.csproj` (`net10.0`). |
| RNF02 | Restrição técnica | O ambiente de desenvolvimento é o Visual Studio Community; o projeto também deve compilar pela linha de comando com `dotnet build`. | Compilação nos dois ambientes. |
| RNF03 | Restrição técnica | No TP1 o sistema é uma aplicação de console standalone, sem orientação a objetos; nas etapas seguintes evoluirá para uma API web com ASP.NET Core. | Inspeção do código por entrega. |
| RNF04 | Persistência | No TP1 os dados ficam apenas em memória durante a execução; a persistência em banco de dados será adicionada em etapa posterior. | Inspeção do código. |
| RNF05 | Usabilidade | A interface de console deve usar menus numerados e textos em português. | Teste manual. |
| RNF06 | Usabilidade | Toda operação recusada deve exibir uma mensagem informando o motivo (ex.: "limite de 3 empréstimos atingido"). | Teste manual de cada regra de recusa. |
| RNF07 | Usabilidade | Datas devem ser exibidas e lidas no formato dd/MM/aaaa e valores monetários em reais com duas casas decimais (R$ 0,00). | Teste manual. |
| RNF08 | Confiabilidade | Entradas inválidas (texto onde se espera número, data inválida, opção inexistente) não podem encerrar o programa; o sistema deve pedir a entrada novamente. | Teste com entradas inválidas. |
| RNF09 | Integridade | Valores monetários devem usar o tipo `decimal`, evitando erros de arredondamento de ponto flutuante. | Inspeção do código. |
| RNF10 | Integridade | ISBN, código de patrimônio e matrícula devem ser únicos no sistema. | Teste de cadastro duplicado. |
| RNF11 | Manutenibilidade | Os parâmetros das regras (prazo de 14 dias, limite de 3 empréstimos, valor diário da multa) devem estar definidos em um único lugar, como constantes. | Inspeção do código. |
| RNF12 | Manutenibilidade | O código-fonte deve ser versionado em repositório Git hospedado no GitHub. | Acesso ao repositório. |
| RNF13 | Desempenho | Consultas ao acervo devem responder em até 2 segundos para um acervo de até 10.000 exemplares. | Teste de carga (a partir da etapa com banco de dados). |
| RNF14 | Segurança | Nas etapas com API, o acesso deve exigir autenticação, e as operações do RF30 devem exigir o papel de Bibliotecário. | Teste de acesso com cada papel. |
| RNF15 | Portabilidade | O sistema deve executar em Windows, Linux e macOS com o runtime do .NET 10 instalado. | Execução em ao menos dois sistemas operacionais. |

## 5. Rastreabilidade

| Regra | Requisitos | Caso de uso |
|-------|------------|-------------|
| RN01 | RF15 | UC02 |
| RN02 | RF21 | UC03 |
| RN03 | RF17 | UC02 |
| RN04 | RF12, RF18, RF22 | UC01, UC02 |
| RN05 | RF18 | UC02 |
| RN06 | RF26 | UC04 |
| RN07 | RF04, RF25 | UC04 |
| RN08 | RF20 | UC03 |
| RN09 | RF21, RF23 | UC03, UC07 |
| RN10 | RF05, RF18, RF22 | UC02, UC05 |
| RN11 | RF01–RF10, RF24, RF30 | UC05, UC06, UC07 |
| RN12 | RF16, RF25 | UC02, UC04 |

As suposições citadas na coluna "Origem" (S01, S02…) estão descritas em [justificativa-escopo.md](justificativa-escopo.md).
