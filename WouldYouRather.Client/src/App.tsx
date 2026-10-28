import MainLayout from "./component/MainLayout";
import Home from "./pages/home";
import { BrowserRouter, Route, Routes } from "react-router-dom";
function App() {
  // Starting point. API client lives in src/api/questionApi.ts, types in
  // src/types/question.ts — build the UI and state from here.
  return (
    <BrowserRouter>
    <Routes>
      <Route element={<MainLayout />}>
        <Route path="/" element={<Home />} />
      </Route>
    </Routes>
    </BrowserRouter>
  )
}

export default App
