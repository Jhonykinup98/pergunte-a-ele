# Pergunte a Ele

Plataforma de perguntas e respostas: qualquer usuário autenticado pode enviar uma pergunta publicamente, e apenas o administrador ("Ele") pode respondê-la. Todos os usuários acompanham o feed completo de perguntas e respostas em tempo real.

## Stack

- **Backend**: C# / ASP.NET Core Web API, Entity Framework Core, PostgreSQL (Supabase)
- **Frontend**: React (Vite)
- **Autenticação**: JWT + login social via Google OAuth (ID Token, validado no backend)
- **Autorização por papel**: usuário comum vs. administrador, via claims no token

## Funcionalidades

- Cadastro com confirmação de e-mail por link
- Login com login/senha ou com Google
- Feed público: qualquer usuário vê todas as perguntas e respostas
- Painel administrativo: apenas o admin responde perguntas pendentes
- Deploy gratuito (Vercel + Render + Supabase)

## Decisões técnicas

- **JWT com claims customizadas** (`login`, `role`) para autorização baseada em papel sem precisar consultar o banco a cada requisição protegida.
- **`RoleClaimType` explícito + `MapInboundClaims = false`** no `Program.cs`, evitando o remapeamento automático de claims que o ASP.NET Core faz por padrão — garante que `User.IsInRole("admin")` funcione de forma previsível.
- **BCrypt** para hash de senha; senha nunca é armazenada nem trafega em texto puro.
- **Validação do login com Google feita no backend** (`Google.Apis.Auth`), não no frontend — o token do Google é sempre verificado no servidor antes de confiar na identidade do usuário.
- **Modo de teste no envio de e-mail**: sem uma API key real configurada, o link de confirmação é logado no console em vez de falhar o cadastro — facilita desenvolvimento local sem depender de um provedor de e-mail configurado.
- **Separação em camadas**: Controllers → Services → Data (EF Core), isolando regra de negócio de infraestrutura.

## Como rodar localmente

### Backend
```bash
cd backend/PergunteAele.Api
cp appsettings.Example.json appsettings.json
# preencha appsettings.json com suas credenciais reais (Supabase, JWT, Google, e-mail)
dotnet restore
dotnet ef database update
dotnet run
```
A API sobe em `https://localhost:5001`. Acesse `/swagger` para testar os endpoints.

### Frontend
```bash
cd frontend/pergunte
cp .env.example .env
# preencha .env com a URL da API e o Google Client ID
npm install
npm run dev
```
Acesse `http://localhost:5173`.

## Configurando o login com Google

1. [Google Cloud Console](https://console.cloud.google.com/) → crie um projeto
2. **APIs & Services → Credentials → Create Credentials → OAuth client ID** (Web application)
3. Em **Authorized JavaScript origins**, adicione `http://localhost:5173` (e a URL de produção, quando houver)
4. Copie o **Client ID** para `appsettings.json` (`Google:ClientId`) e para `.env` (`VITE_GOOGLE_CLIENT_ID`)

## Deploy

| Camada | Serviço |
|---|---|
| Frontend | Vercel — conecta o repo, aponta pra pasta `frontend/pergunte` |
| Backend | Render — detecta o `Dockerfile` automaticamente, variáveis de ambiente equivalentes ao `appsettings.json` |
| Banco | Supabase (PostgreSQL gerenciado) |
| E-mail | Resend |


Login

<img width="1919" height="933" alt="Index" src="https://github.com/user-attachments/assets/220abae3-049f-4d7a-9024-0dc6bb30a17c" />

<img width="1915" height="915" alt="Login" src="https://github.com/user-attachments/assets/1a1045e5-8fad-4045-b3fa-ca8aefe74866" />


Cadastro

<img width="1915" height="908" alt="Cadastro" src="https://github.com/user-attachments/assets/c52b50e7-f628-4441-871c-1f3d7c59e8ad" />

Login usuário

<img width="1918" height="933" alt="LoginUsuario" src="https://github.com/user-attachments/assets/7b3d970c-4840-4427-8b16-a4aa748d5245" />

Login ele 

<img width="1919" height="933" alt="LoginELE" src="https://github.com/user-attachments/assets/f7e14772-a861-42d2-9dce-8e4e91ea9fa3" />


## Sobre o projeto

Este projeto foi desenvolvido por iniciativa própria, apenas gostaria de por em prática algo que estava em mente.

