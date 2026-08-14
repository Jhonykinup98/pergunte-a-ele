# PergunteAele

Projeto com backend em C# (ASP.NET Core Web API) e frontend em React (Vite).
Autenticação por login/senha com confirmação de e-mail, e login social via Google.

## Estrutura

```
PergunteAele/
├── backend/PergunteAele.Api/     -> API em C#
└── frontend/pergunte-ael-web/    -> App em React
```

## 1. Rodando o backend localmente

```bash
cd backend/PergunteAele.Api
dotnet restore
dotnet run
```

A API sobe em `https://localhost:5001` (ou porta similar, o terminal mostra).
Acesse `/swagger` para testar os endpoints (`/auth/register`, `/auth/login`, etc).

### Antes de rodar, configure o `appsettings.json`:
- `ConnectionStrings:DefaultConnection` → pegue no Supabase (Project Settings > Database > Connection string).
- `Jwt:Key` → qualquer string grande e aleatória.
- `Google:ClientId` → criado no Google Cloud Console (veja passo 4).
- `Email:ApiKey` → chave da API do Resend (ou troque o EmailService por SendGrid).

### Criar as tabelas no banco (EF Core Migrations):

```bash
dotnet tool install --global dotnet-ef   # só na primeira vez
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## 2. Rodando o frontend localmente

```bash
cd frontend/pergunte-ael-web
npm install
cp .env.example .env    # depois edite com suas chaves
npm run dev
```

Acesse `http://localhost:5173`.

## 3. Deploy gratuito

| Camada | Serviço | Como |
|---|---|---|
| Frontend | Vercel | Conecte o repositório do GitHub (pasta `frontend/pergunte-ael-web`), configure as env vars `VITE_API_URL` e `VITE_GOOGLE_CLIENT_ID` no painel. |
| Backend | Render | "New Web Service" → conecte o repo → aponte para a pasta `backend/PergunteAele.Api` → Render detecta o `Dockerfile` automaticamente. Configure as env vars lá (mesmas chaves do `appsettings.json`, como variáveis de ambiente). |
| Banco | Supabase | Crie um projeto grátis → copie a connection string do Postgres. |
| E-mail | Resend | Crie conta grátis → gere uma API Key → confirme um domínio (ou use o domínio de teste deles para começar). |

## 4. Configurar o login com Google

1. Acesse [Google Cloud Console](https://console.cloud.google.com/) → crie um projeto.
2. Vá em "APIs & Services" → "Credentials" → "Create Credentials" → "OAuth client ID".
3. Tipo de aplicação: "Web application".
4. Em "Authorized JavaScript origins", adicione:
   - `http://localhost:5173`
   - a URL da Vercel (ex: `https://pergunte-ael-web.vercel.app`)
5. Copie o **Client ID** gerado e cole em:
   - `appsettings.json` → `Google:ClientId` (backend)
   - `.env` → `VITE_GOOGLE_CLIENT_ID` (frontend)

## Fluxo implementado

1. **Cadastro** (`/cadastro`) → login, gmail, senha, repetir senha → envia e-mail de confirmação.
2. **Confirmação** (`/confirmar-email?token=...`) → ativa a conta.
3. **Login** (`/login`) → por login/senha OU pelo botão "Entrar com Google".
4. **Dashboard** (`/dashboard`) → rota protegida, só acessível logado.
