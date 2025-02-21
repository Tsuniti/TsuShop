import {createRoot} from 'react-dom/client'

import {BrowserRouter, Route, Routes} from "react-router-dom";
import HomePage from "./pages/HomePage/HomePage.jsx";
import Header from "./components/Header.jsx";
import MyFooter from "./components/MyFooter.jsx";
import ShopPage from "./pages/ShopPage/ShopPage.jsx";
import PageNotFound from "./pages/PageNotFound/PageNotFound.jsx";
import {CategoryProvider} from "./functions/CategoryContext.jsx";
import AdminPage from "./pages/AdminPage/AdminPage.jsx";
import LoginPage from "./pages/LoginPage/LoginPage.jsx";
import RegisterPage from "./pages/RegisterPage/RegisterPage.jsx";
import ProductPage from "./pages/ProductPage/ProductPage.jsx";
import CartPage from "./pages/CartPage/CartPage.jsx";


createRoot(document.getElementById('root')).render(
    <BrowserRouter>
        <CategoryProvider>
            <Header/>
            <Routes>
                <Route path="/" element={<HomePage/>}/>
                <Route path="/products" element={<ShopPage/>}/>
                <Route path="/login" element={<LoginPage/>}/>
                <Route path="/register" element={<RegisterPage/>}/>
                <Route path="/products/:productId" element={<ProductPage />} />
                <Route path="/cart" element={<CartPage/>}/>


                <Route path="/admin-panel" element={<AdminPage/>}/>


                <Route path="/*" element={<PageNotFound/>}/>
            </Routes>
            <MyFooter/>
        </CategoryProvider>
    </BrowserRouter>
)
