# realemmetts Task List - Frontend (Dev B)

**Story:** Authenticated App Shell (5 SP)
**Sprint:** 1 | Jan 4–17, 2026
**Tech Stack:** React 18, TypeScript, Vite, Tailwind CSS
**Branch:** `feature/story1-frontend-auth`

---

## PHASE 0: API CONTRACT (SYNC POINT - DO FIRST)

Before coding, confirm these contracts with cadleta:

### Auth Endpoints (cadleta Builds These)
| Method | Endpoint | Request Body | Response |
|--------|----------|--------------|----------|
| POST | /api/auth/register | { email, password, displayName } | { userId, email, token } |
| POST | /api/auth/login | { email, password } | { userId, email, token, expiresAt } |
| POST | /api/auth/logout | (none, uses auth header) | { success: true } |
| GET | /api/auth/me | (none, uses auth header) | { userId, email, displayName, points } |

### JWT Claims
- `sub` (user ID)
- `email`
- `name` (display name)
- `exp` (expiration timestamp)

### Error Response Format
```json
{ "error": "string", "details": ["string"] }
```

---

## YOUR TASKS

### B1. Scaffold React + Vite + TypeScript Project
**Directory:** `bigjobhunterpro-web/`
- [ ] Create Vite project with React + TypeScript template
- [ ] Install dependencies (react-router-dom, axios, etc.)
- [ ] Configure TypeScript strict mode
- [ ] Add .gitignore for Node

**Commands:**
```bash
npm create vite@latest bigjobhunterpro-web -- --template react-ts
cd bigjobhunterpro-web
npm install
npm install react-router-dom axios
npm install -D @types/node
```

**Files created:**
```
bigjobhunterpro-web/
├── src/
│   ├── App.tsx
│   ├── main.tsx
│   └── vite-env.d.ts
├── index.html
├── package.json
├── tsconfig.json
└── vite.config.ts
```

---

### B2. Configure Tailwind CSS with Retro Theme
**Depends on:** B1
- [ ] Install Tailwind CSS + PostCSS + Autoprefixer
- [ ] Configure tailwind.config.js with theme colors
- [ ] Add "Press Start 2P" font for headers
- [ ] Add Inter font for body text

**Commands:**
```bash
npm install -D tailwindcss postcss autoprefixer
npx tailwindcss init -p
```

**Theme Colors (tailwind.config.js):**
```javascript
theme: {
  extend: {
    colors: {
      'forest': '#2E4600',
      'blaze': '#FF6700',
      'amber': '#FFB000',
      'terminal': '#00FF00',
      'dark': '#0a0a0a',
    },
    fontFamily: {
      'arcade': ['"Press Start 2P"', 'cursive'],
      'body': ['Inter', 'sans-serif'],
    },
  },
}
```

**Files to create:**
- `tailwind.config.js`
- `postcss.config.js`
- `src/index.css` (Tailwind directives + custom styles)

**Add to index.css:**
```css
@import url('https://fonts.googleapis.com/css2?family=Press+Start+2P&family=Inter:wght@400;500;600;700&display=swap');
@tailwind base;
@tailwind components;
@tailwind utilities;
```

---

### B3. Build App Shell Layout with Nav Placeholders
**Depends on:** B2
- [ ] Create AppShell component with header, sidebar placeholder, main content area
- [ ] Add logo/title in header
- [ ] Add placeholder nav links (Dashboard, Applications, Party, Profile)
- [ ] Add user menu dropdown (placeholder)
- [ ] Style with retro-arcade theme

**Files to create:**
- `src/components/layout/AppShell.tsx`
- `src/components/layout/Header.tsx`
- `src/components/layout/NavLink.tsx`

**Structure:**
```
src/components/layout/
├── AppShell.tsx      # Main wrapper with header + content area
├── Header.tsx        # Top bar with logo, nav, user menu
└── NavLink.tsx       # Reusable nav link component
```

---

### B4. Configure React Router with Protected Routes
**Depends on:** B3
- [ ] Create ProtectedRoute wrapper component
- [ ] Define routes:
  - /login (public)
  - /register (public)
  - /app/* (protected, uses AppShell)
  - /app/dashboard (placeholder)
- [ ] Redirect unauthenticated users to /login
- [ ] Redirect authenticated users away from /login

**Files to create:**
- `src/router/index.tsx`
- `src/router/ProtectedRoute.tsx`
- `src/pages/Dashboard.tsx` (placeholder)

**Route Structure:**
```tsx
<Routes>
  <Route path="/login" element={<Login />} />
  <Route path="/register" element={<Register />} />
  <Route path="/app" element={<ProtectedRoute><AppShell /></ProtectedRoute>}>
    <Route path="dashboard" element={<Dashboard />} />
    <Route index element={<Navigate to="dashboard" />} />
  </Route>
  <Route path="*" element={<Navigate to="/app" />} />
</Routes>
```

---

### B5. Build Login Page with Validation
**Depends on:** B4
- [ ] Create Login page component
- [ ] Email input with validation (required, email format)
- [ ] Password input with validation (required, min 8 chars)
- [ ] Submit button with loading state
- [ ] Inline error messages (display within 100ms)
- [ ] Link to Register page
- [ ] Handle API errors gracefully

**Files to create:**
- `src/pages/Login.tsx`
- `src/components/forms/FormInput.tsx`
- `src/components/forms/FormError.tsx`

**Validation Rules:**
- Email: required, valid email format
- Password: required, min 8 characters

---

### B6. Build Register Page with Validation
**Depends on:** B5
- [ ] Create Register page component
- [ ] Display name input (required)
- [ ] Email input with validation
- [ ] Password input with validation
- [ ] Confirm password with match validation
- [ ] Submit button with loading state
- [ ] Link to Login page
- [ ] Auto-login after successful registration

**Files to create:**
- `src/pages/Register.tsx`

**Validation Rules:**
- Display name: required, min 2 characters
- Email: required, valid email format
- Password: required, min 8 characters
- Confirm password: must match password

---

### B7. Implement API Client with Token Storage
**Depends on:** B1
- [ ] Create axios instance with base URL config
- [ ] Add request interceptor to attach JWT from storage
- [ ] Add response interceptor for 401 handling (redirect to login)
- [ ] Store token in localStorage
- [ ] Create auth service functions (login, register, logout, getMe)

**Files to create:**
- `src/services/api.ts`
- `src/services/auth.ts`
- `src/hooks/useAuth.ts`
- `src/context/AuthContext.tsx`

**API Client Structure:**
```typescript
// src/services/api.ts
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL || 'https://localhost:5001',
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});
```

**Environment Variable:**
Create `.env` file:
```
VITE_API_URL=https://localhost:5001
```

---

### B8. Implement Logout Flow
**Depends on:** B7
- [ ] Add logout button to AppShell header
- [ ] Clear token from storage on logout
- [ ] Redirect to /login after logout
- [ ] (Optional) Call /api/auth/logout to invalidate server-side

**Files to modify:**
- `src/components/layout/Header.tsx`
- `src/hooks/useAuth.ts`

---

## INTEGRATION CHECKLIST (with cadleta)

When cadleta's backend is ready:
- [ ] Update VITE_API_URL in .env to point to Dev A's backend
- [ ] Test: Register new user -> verify in database
- [ ] Test: Login with registered user -> receive valid JWT
- [ ] Test: Access /app/dashboard -> displays AppShell
- [ ] Test: Access /app without token -> redirects to /login
- [ ] Test: Logout -> clears token, redirects to /login
- [ ] Test: Refresh page while logged in -> stays authenticated

---

## COMPLETION CRITERIA

- [ ] Login page with email/password validation
- [ ] Register page with form validation
- [ ] Protected routes redirect to /login when unauthenticated
- [ ] App shell renders with navigation placeholders
- [ ] Logout clears token and redirects to /login
- [ ] Token persists across page refresh

---

## GIT WORKFLOW

**Your Branch:** `feature/story1-frontend-auth`

```bash
git checkout -b feature/story1-frontend-auth
# ... do your work ...
git add .
git commit -m "feat: add login/register pages with auth context"
git push -u origin feature/story1-frontend-auth
```

**Commit Convention:** `feat:`, `fix:`, `refactor:`, `docs:`

**Merge Order:** cadleta merges backend first, then you merge frontend (no conflicts expected).

---

## DEV SERVER

```bash
cd bigjobhunterpro-web
npm run dev
```

Opens at http://localhost:5173

---

## FILE STRUCTURE (Final)

```
bigjobhunterpro-web/
├── src/
│   ├── components/
│   │   ├── forms/
│   │   │   ├── FormInput.tsx
│   │   │   └── FormError.tsx
│   │   └── layout/
│   │       ├── AppShell.tsx
│   │       ├── Header.tsx
│   │       └── NavLink.tsx
│   ├── context/
│   │   └── AuthContext.tsx
│   ├── hooks/
│   │   └── useAuth.ts
│   ├── pages/
│   │   ├── Dashboard.tsx
│   │   ├── Login.tsx
│   │   └── Register.tsx
│   ├── router/
│   │   ├── index.tsx
│   │   └── ProtectedRoute.tsx
│   ├── services/
│   │   ├── api.ts
│   │   └── auth.ts
│   ├── App.tsx
│   ├── main.tsx
│   └── index.css
├── .env
├── tailwind.config.js
├── postcss.config.js
├── tsconfig.json
├── vite.config.ts
└── package.json
```
