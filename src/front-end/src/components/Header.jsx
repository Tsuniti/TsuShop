import {useNavigate} from "react-router-dom";
import {useCategories} from "../functions/CategoryContext.jsx";
import {useEffect, useState} from "react";
import { jwtDecode } from "jwt-decode";
import Config from "../config.jsx";


function Header() {



    const categories = useCategories();

    const navigate = useNavigate();
    const [isCollapsed, setIsCollapsed] = useState(true); // Состояние для контроля видимости меню
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isUserAdmin, setIsUserAdmin] = useState(false);
    const [searchTerm, setSearchTerm] = useState("");



    // Проверка токена при монтировании компонента
    useEffect(() => {
        const token = localStorage.getItem("authToken");
        //console.log(token);

        if (token) {
            try {
                // Декодируем токен и проверяем его срок действия
                const decodedToken = jwtDecode(token);
                const expirationTime = decodedToken.exp * 1000; // Преобразуем в миллисекунды
                //console.log(decodedToken);
                //console.log(Date.now() < expirationTime)
                if (Date.now() < expirationTime) {
                    setIsAuthenticated(true);  // Токен действителен
                    setIsUserAdmin(decodedToken.IsAdmin);
                    //console.log(decodedToken.IsAdmin);
                    //console.log(isUserAdmin);
                } else {
                    console.log('expired token: '+token);
                    localStorage.removeItem("authToken");  // Токен просрочен, удаляем его
                    setIsAuthenticated(false);
                    setIsUserAdmin(false);
                }
            } catch (error) {
                console.log('token error: '+error);
                localStorage.removeItem("authToken");  // Ошибка в декодировании токена, удаляем его
                setIsAuthenticated(false);
                setIsUserAdmin(false);
            }
        }
    });



    const handleLogout = () => {
        // Удаляем токен из localStorage
        localStorage.removeItem("authToken");
        setIsAuthenticated(false);
        setIsUserAdmin(false);
        // Перенаправляем пользователя на страницу логина
        navigate("/login");
    };
    const handleLogin = () => {
        navigate("/login");  // Перенаправляем на страницу логина
    };
    const handleRegister = () => {
        navigate("/register");  // Перенаправляем на страницу логина
    };


    useEffect(() => {
        // Инициализация collapse компонента после рендера, чтобы он был закрыт по умолчанию
        const collapseElement = document.getElementById("navbar-vertical");
        const bsCollapse = new window.bootstrap.Collapse(collapseElement, { toggle: false }); // Не открывать автоматически
        bsCollapse.hide(); // Скрыть меню при монтировании компонента
    }, []);

    const handleCategoryClick = (selectedCategory) => {
        navigate("/products", { state: { selectedCategory } });
        // Скрыть меню после клика
        setIsCollapsed(true);
    };

    const toggleMenu = () => {
        setIsCollapsed(!isCollapsed); // Меняем состояние при клике на кнопку
    };

    const handleSearchChange = (e) => {
        setSearchTerm(e.target.value);  // Обновляем состояние при вводе текста
    };

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        navigate(`/products?search=${searchTerm}`);  // Перенаправляем на /products с query параметром
    };


    return (

        <div>
            {/*<!-- Topbar Start -->*/}
            <div className="container-fluid">
                <div className="row bg-secondary py-1 px-xl-5">
                    <div className="col-lg-6 d-none d-lg-block">
                        <div className="d-inline-flex align-items-center h-100">

                            {"True" == isUserAdmin && (
                            <a className="text-body mr-3" href="/admin-panel">Admin panel</a>
                            )}
                        </div>
                    </div>
                    <div className="col-lg-6 text-center text-lg-right">
                        {!isAuthenticated ? (
                            <div>
                            <a
                                href="/login"
                                className="text-body mr-3"
                                style={{ fontSize: "20px" }}
                            >
                                Login
                            </a>

                            <a
                                href="/register"
                                className="text-body"
                                style={{ fontSize: "20px" }}
                            >
                                Register
                            </a>
                            </div>
                        ) : (
                            <a
                                href="#"
                                className="text-body"
                                style={{ fontSize: "20px" }}
                                onClick={handleLogout}
                            >
                                Logout
                            </a>
                        )}
                    </div>
                </div>
                <div className="row align-items-center bg-light py-3 px-xl-5 d-none d-lg-flex">
                    <div className="col-lg-4">
                        <a href="/" className="text-decoration-none">
                            <span className="h1 text-uppercase text-primary bg-dark px-2">Tsu</span>
                            <span className="h1 text-uppercase text-dark bg-primary px-2 ml-n1">Shop</span>
                        </a>
                    </div>
                    <div className="col-lg-4 col-6 text-left">
                        <form onSubmit={handleSearchSubmit}>
                            <div className="input-group">
                                <input type="text"
                                       className="form-control"
                                       placeholder="Search for products"
                                       value={searchTerm}
                                       onChange={handleSearchChange}/>
                                    <div className="input-group-append">
                            <button type="submit" className="input-group-text bg-transparent text-primary">
                                <i className="fa fa-search"/>
                            </button>
                                    </div>
                            </div>
                        </form>
                    </div>
                    <div className="col-lg-4 col-6 text-right">
                        <p className="m-0">Customer Service</p>
                        <h5 className="m-0">{Config['phone']}</h5>
                    </div>
                </div>
            </div>
            {/*<!-- Topbar End -->*/}


            {/*<!-- Navbar Start -->*/}
            <div className="container-fluid bg-dark mb-30">
                <div className="row px-xl-5">
                    <div className="col-lg-3 d-none d-lg-block">
                        <a className="btn d-flex align-items-center justify-content-between bg-primary w-100"
                           data-toggle="collapse"
                           onClick={toggleMenu} style={{height: '65px', padding: '0 30px'}}>
                            <h6 className="text-dark m-0"><i className="fa fa-bars mr-2"></i>Categories</h6>
                            <i className="fa fa-angle-down text-dark"></i>
                        </a>
                        <nav
                            className={`collapse position-absolute navbar navbar-vertical navbar-light align-items-start p-0 bg-light ${isCollapsed ? '' : 'show'}`}
                            id="navbar-vertical" style={{width: 'calc(100% - 30px)', zIndex: '999'}}>
                            <div className="navbar-nav w-100">

                                {categories.map((category) => (
                                    <button key={category} onClick={() => handleCategoryClick(category)}
                                         className="nav-item nav-link btn btn-light"
                                            style={{ textAlign: "left", width: '100%'}}
                                        >{category}
                                    </button>
                                ))}
                            </div>
                        </nav>
                    </div>
                    <div className="col-lg-9">
                        <nav className="navbar navbar-expand-lg bg-dark navbar-dark py-3 py-lg-0 px-0">
                            <button type="button" className="navbar-toggler" data-toggle="collapse"
                                    data-target="#navbarCollapse">
                                <span className="navbar-toggler-icon"></span>
                            </button>
                            <div className="collapse navbar-collapse justify-content-between" id="navbarCollapse">
                                <div className="navbar-nav mr-auto py-0">
                                    <a href="/" className="nav-item nav-link">Home</a>
                                    <a href={"/products"} className="nav-item nav-link">Shop</a>

                                </div>
                                { isAuthenticated &&
                                <div className="navbar-nav ml-auto py-0 d-none d-lg-block">
                                    <a href="/cart" className="btn px-0 ml-3">
                                        <i className="fas fa-shopping-cart text-primary"></i>

                                    </a>
                                </div>
                                }
                            </div>
                        </nav>
                    </div>
                </div>
            </div>
            {/*<!-- Navbar End -->*/}
        </div>
)
}
export default Header;