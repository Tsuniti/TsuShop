import OneProductTile from "../components/OneProductTile.jsx";
import {useEffect, useState} from "react";
import Path from "../../endpoints.jsx";
import {useCategories} from "../../functions/CategoryContext.jsx";
import {useNavigate} from "react-router-dom";

function HomePage() {

    const categories = useCategories();

    const navigate = useNavigate();
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetch(`${Path['product']}?quantity=8`)
            .then((response) => response.json())
            .then((data) => {
                setProducts(data.products);
            })
            .catch((error) => console.error("Error fetching data:", error));
    }, []);

    const handleCategoryClick = (selectedCategory) => {
        navigate("/products", { state: { selectedCategory } });
        // Скрыть меню после клика
    };



    return (
        <div>
            {/*<!-- Carousel Start -->*/}
            <div className="container-fluid mb-3">
                <div className="row px-xl-5">
                    <div className="col">
                        <div id="header-carousel" className="carousel slide carousel-fade mb-30 mb-lg-0" data-ride="carousel">
                            <ol className="carousel-indicators">
                                <li data-target="#header-carousel" data-slide-to="0" className="active"></li>
                                <li data-target="#header-carousel" data-slide-to="1"></li>
                                <li data-target="#header-carousel" data-slide-to="2"></li>
                            </ol>
                            <div className="carousel-inner">
                                <div className="carousel-item position-relative active" style={{ height: '430px'}}>
                                    <img className="position-absolute w-100 h-100" src="/assets/img/computer-accessories.jpg" style={{objectFit: 'cover'}}/>
                                        <div className="carousel-caption d-flex flex-column align-items-center justify-content-center">
                                            <div className="p-3" style={{maxWidth: '700px'}}>
                                                <h1 className="display-4 text-white mb-3 animate__animated animate__fadeInDown">Computer accessories</h1>
                                                <p className="mx-md-5 px-5 animate__animated animate__bounceIn">High-quality peripherals designed to enhance your computing experience with precision and style</p>
                                                <a className="btn btn-outline-light py-2 px-4 mt-3 animate__animated animate__fadeInUp"
                                                   onClick={() => handleCategoryClick("Computer accessories")}
                                                >Shop Now</a>
                                            </div>
                                        </div>
                                </div>
                                <div className="carousel-item position-relative" style={{height: '430px'}}>
                                    <img className="position-absolute w-100 h-100" src="/assets/img/consoles.png" style={{objectFit: 'cover'}}/>
                                        <div className="carousel-caption d-flex flex-column align-items-center justify-content-center">
                                            <div className="p-3" style={{maxWidth: '700px'}}>
                                                <h1 className="display-4 text-white mb-3 animate__animated animate__fadeInDown">Consoles</h1>
                                                <p className="mx-md-5 px-5 animate__animated animate__bounceIn">Next-gen gaming consoles offering unparalleled performance and immersive gameplay for enthusiasts</p>
                                                <a className="btn btn-outline-light py-2 px-4 mt-3 animate__animated animate__fadeInUp"
                                                   onClick={() => handleCategoryClick("Consoles")}>Shop Now</a>
                                            </div>
                                        </div>
                                </div>
                                <div className="carousel-item position-relative" style={{height: '430px'}}>
                                    <img className="position-absolute w-100 h-100" src="/assets/img/smartphones.png" style={{objectFit: 'cover'}}/>
                                        <div className="carousel-caption d-flex flex-column align-items-center justify-content-center">
                                            <div className="p-3" style={{maxWidth: '700px'}}>
                                                <h1 className="display-4 text-white mb-3 animate__animated animate__fadeInDown">Smartphones</h1>
                                                <p className="mx-md-5 px-5 animate__animated animate__bounceIn">Smartphones with cutting-edge technology, providing seamless connectivity and advanced features for all users</p>
                                                <a className="btn btn-outline-light py-2 px-4 mt-3 animate__animated animate__fadeInUp"
                                                   onClick={() => handleCategoryClick("Smartphones")}>Shop Now</a>
                                            </div>
                                        </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            {/*<!-- Carousel End -->*/}


            {/*<!-- Featured Start -->*/}
            <div className="container-fluid pt-5">
                <div className="row px-xl-5 pb-3">
                    <div className="col-lg-3 col-md-6 col-sm-12 pb-1">
                        <div className="d-flex align-items-center bg-light mb-4" style={{padding: '30px'}}>
                            <h1 className="fa fa-check text-primary m-0 mr-3"></h1>
                            <h5 className="font-weight-semi-bold m-0">Quality Product</h5>
                        </div>
                    </div>
                    <div className="col-lg-3 col-md-6 col-sm-12 pb-1">
                        <div className="d-flex align-items-center bg-light mb-4" style={{padding: '30px'}}>
                            <h1 className="fa fa-shipping-fast text-primary m-0 mr-2"></h1>
                            <h5 className="font-weight-semi-bold m-0">Free Shipping</h5>
                        </div>
                    </div>
                    <div className="col-lg-3 col-md-6 col-sm-12 pb-1">
                        <div className="d-flex align-items-center bg-light mb-4" style={{padding: '30px'}}>
                            <h1 className="fas fa-exchange-alt text-primary m-0 mr-3"></h1>
                            <h5 className="font-weight-semi-bold m-0">14-Day Return</h5>
                        </div>
                    </div>
                    <div className="col-lg-3 col-md-6 col-sm-12 pb-1">
                        <div className="d-flex align-items-center bg-light mb-4" style={{padding: '30px'}}>
                            <h1 className="fa fa-phone-volume text-primary m-0 mr-3"></h1>
                            <h5 className="font-weight-semi-bold m-0">24/7 Support</h5>
                        </div>
                    </div>
                </div>
            </div>
            {/*<!-- Featured End -->*/}


            {/*<!-- Products Start -->*/}
            <div className="container-fluid pt-5 pb-3">
                <h2 className="section-title position-relative text-uppercase mx-xl-5 mb-4"><span className="bg-secondary pr-3">Recent Products</span></h2>
                <div className="row px-xl-5">
                    {products.map((product) => (
                        <OneProductTile key={product.id} product={product} size={"small"} />
                    ))}

                </div>
            </div>
            {/*<!-- Products End -->*/}


        </div>
    )
}

export default HomePage;