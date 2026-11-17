# Justificativa de Escopo — TP1

- **Aluno:** Uanderson Bueno
- **Disciplina:** 26E4_5 — Projeto de Bloco
- **Entrega:** TP1

## 1. Visão geral do recorte

O projeto vai evoluir ao longo do bloco: de um programa de console até uma API web com banco de dados e autenticação. Por isso o TP1 foi dividido em duas partes com escopos diferentes:

- **Análise e modelagem: escopo completo.** Os requisitos e casos de uso cobrem o sistema inteiro descrito na especificação, porque este documento serve de referência para todas as entregas seguintes. Modelar só uma parte agora obrigaria a refazer a análise em cada TP.
- **Código: escopo mínimo.** O enunciado pede um programa simples, **sem orientação a objetos**, aplicando tipos primitivos, operadores e controle de fluxo. Implementar o sistema inteiro dessa forma geraria um código difícil de manter, que seria jogado fora no TP2, quando entram as classes. Por isso o programa cobre só o núcleo das regras de circulação.

## 2. Dentro do escopo do TP1

| Item | Onde |
|------|------|
| Documento de requisitos completo (30 RFs, 15 RNFs, 12 RNs) | `docs/requisitos.md` |
| Diagrama de casos de uso com 2 atores, 7 casos principais e 3 auxiliares (include/extend) | `docs/diagrama-casos-de-uso.svg` |
| Especificação detalhada de 3 cenários (UC02, UC03, UC04) com fluxos alternativos, mais UC01 resumido | `docs/casos-de-uso.md` |
| Ambiente .NET 10 + Visual Studio Community configurado | `BibliotecaUniversitaria.slnx` |
| Programa de console com acervo fixo de exemplo e as regras RN01–RN06, RN08–RN10 e RN12 | `src/BibliotecaUniversitaria/Program.cs` |

**O que o programa faz:** consultar o acervo e a disponibilidade, realizar empréstimo (com validação de cadastro, limite de 3, estado do exemplar e multa pendente), registrar devolução (com cálculo de atraso e multa), realizar reserva (só para livro com exemplar emprestado) e pagar multas.

**Como o programa foi construído:** variáveis de tipos primitivos (`int`, `bool`, `decimal`, `string`), vetores, operadores aritméticos, relacionais e lógicos, `if`/`else`, `switch`, `while` e `for`, além de funções locais. Não há classes próprias, `struct`s nem coleções genéricas. Os dados de exemplo ficam em vetores paralelos (o índice `i` de cada vetor descreve o mesmo exemplar). As datas usam o tipo `DateOnly` da biblioteca padrão, só para ler e somar datas.

## 3. Deixado deliberadamente para depois

| Item | Motivo | Etapa prevista |
|------|--------|----------------|
| Classes `Livro`, `Exemplar`, `Membro`, `Emprestimo`, `Reserva`, `Multa` | O enunciado proíbe OO no TP1. Os vetores paralelos do programa já mostram onde cada classe vai surgir. | Programa OO |
| Cadastro, alteração e remoção de livros, exemplares e membros (UC05, UC06) | Com vetores de tamanho fixo e sem OO, o CRUD seria só código repetitivo. O acervo de exemplo é fixo. | Programa OO |
| Persistência dos dados | Os dados ficam em memória (RNF04). Ao fechar o programa, tudo volta ao estado inicial. | Etapa de banco de dados |
| Login e controle de acesso por papel (RF30, RNF14) | No console, quem opera é sempre o bibliotecário. A separação de papéis será feita com autenticação na API. | Etapa de autenticação |
| Cancelamento e listagem administrativa de reservas (RF28) | É secundário ao fluxo principal. O programa já registra as reservas e avisa a mais antiga na devolução (RF29). | Programa OO |
| Interface web / API | Fora do objetivo do TP1. | Etapa ASP.NET Core |

## 4. Suposições feitas ao interpretar a especificação

| ID | Ponto em aberto na especificação | Suposição adotada |
|----|----------------------------------|-------------------|
| S01 | O valor da multa por dia não é informado. | **R$ 1,00 por dia de atraso**, definido em uma constante para facilitar mudanças (RNF11). |
| S02 | Não diz se fins de semana e feriados contam. | "Dias corridos" inclui fins de semana e feriados. O prazo não é prorrogado se cair em dia sem expediente. |
| S03 | Não diz se um membro com multa pendente pode pegar livros. | Membro com multa **pendente** não pode realizar novos empréstimos até quitá-la (RF19). É a prática comum em bibliotecas e dá sentido à "situação de pagamento" da multa. |
| S04 | Os tipos de membro (aluno, professor, funcionário) não têm regras diferentes. | Todos os tipos seguem o mesmo prazo (14 dias) e o mesmo limite (3 empréstimos). O tipo fica registrado para uso futuro. |
| S05 | Não diz se um membro pode reservar o mesmo livro mais de uma vez. | Um membro pode ter só **uma reserva ativa por livro** (RF27). |
| S06 | Não define a ordem de atendimento das reservas. | As reservas são atendidas por **ordem de data** (a primeira a ser feita é a primeira atendida). |
| S07 | "Cadastro válido" não é definido. | Cadastro válido = membro cadastrado com situação **ativa**. Membros desligados da universidade ficam inativos. |
| S08 | Não diz se devolver no dia do prazo gera multa. | Devolver **até o último dia do prazo, inclusive**, não gera multa. Atraso = data efetiva − prazo, contado só quando positivo. |
| S09 | RN06 permite reserva se "possua exemplares atualmente emprestados". Não fica claro se isso exige que **todos** estejam emprestados. | Interpretação literal: basta **pelo menos um** exemplar emprestado. Só é recusada quando todos estão disponíveis, como diz o texto. Este ponto deve ser confirmado com o professor. |
| S10 | "Membro solicita empréstimos e devoluções" e "Bibliotecário processa empréstimos e devoluções". | O membro **solicita** e o bibliotecário **registra** no sistema. Por isso os dois atores aparecem em UC02 e UC03. |
| S11 | Os valores possíveis do estado de conservação não são definidos. | Três estados: **bom** (pode emprestar), **danificado** e **indisponível** (extraviado, em restauro etc.). Os dois últimos bloqueiam o empréstimo (RN10). |
| S12 | Remoção de registros com vínculos ativos. | Não se pode remover livro, exemplar ou membro que tenha empréstimo ativo ou multa pendente, para não perder o histórico. |
| S13 | Renovação de empréstimo não é mencionada. | Fora do escopo. Para estender o prazo, é preciso devolver e emprestar de novo. |
| S14 | Multa de devolução parcial ou exemplar perdido. | Fora do escopo do TP1. Um exemplar perdido é marcado como "indisponível" pelo bibliotecário. |
