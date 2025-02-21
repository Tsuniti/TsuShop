import CatalogPriceFilter from "./components/DualRangeSlider/CatalogPriceFilter.jsx";
import NavPages from "../components/NavPages/NavPages.jsx";
import {useEffect, useState} from "react";
import OneProductTile from "../components/OneProductTile.jsx";
import Path from "../../endpoints.jsx";
import {useLocation} from "react-router-dom";
import {useCategories} from "../../functions/CategoryContext.jsx";


function ShopPage() {

    const [priceRange, setPriceRange] = useState({min: 0, max: 1000});
    const [products, setProducts] = useState([]);
    const [maxPages, setMaxPages] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [productsOnPage, setProductsOnPage] = useState(20);
    const [sortBy, setSortBy] = useState("CreatedAt")
    const [isAscending, setIsAscending] = useState(true);
    const [maxPrice, setMaxPrice] = useState(null);
    const [selectedCategory, setSelectedCategory] = useState("");
    const categories = useCategories();

    const location = useLocation();

    const queryParams = new URLSearchParams(location.search);
    const searchTerm = queryParams.get("search");




    useEffect(() => {
        if (location.state?.selectedCategory) {
            setSelectedCategory(location.state.selectedCategory);
        }
    }, [location.state]);

    const handlePriceChange = (newRange) => {
        setPriceRange(newRange);
        //console.log("Выбранный диапазон:", newRange);
    };

    useEffect(() => {
        fetch(`${Path['productsMaxPrice']}`)
            .then((response) => response.json())
            .then((data) => {
                setMaxPrice(data);
            })
            .catch((error) => console.error("Error fetching data:", error));
        //console.log(maxPrice);
    });

    useEffect(() => {
        fetch(`${Path['product']}` +
            `?quantity=${productsOnPage}` +
            `&sortBy=${sortBy}` +
            `&isAscending=${isAscending}` +
            `&minPrice=${priceRange.min}` +
            `&maxPrice= ${priceRange.max}` +
            `&page=${currentPage}` +
            `${selectedCategory ? `&category=${selectedCategory}` : ''}`+
            `${searchTerm ? `&name=${searchTerm}` : ''}`)
            .then((response) => response.json())
            .then((data) => {
                setProducts(data.products);
                setMaxPages(data.pages);
            })
            .catch((error) => console.error("Error fetching data:", error));

        //console.log(products);

    }, [productsOnPage, sortBy, isAscending, priceRange, currentPage, selectedCategory, searchTerm]);

    useEffect(() => {
        setCurrentPage(1);
    }, [productsOnPage, sortBy, isAscending, priceRange, selectedCategory, searchTerm])


    const handleCategoryChange = (e) => {
        setSelectedCategory(e.target.value); // Обновляем состояние при выборе новой категории
    };


    const handleProductsOnPageChange = (e) => {
        setProductsOnPage(e.target.value); // Обновляем состояние выбранного значения
    };


    return (
        <div>


            {/*<!-- Shop Start -->*/}
            <div className="container-fluid">
                <div className="row px-xl-5">
                    {/*<!-- Shop Sidebar Start -->*/}
                    <div className="col-lg-3 col-md-4">
                        {/*<!-- Price Start -->*/}
                        <h5 className="section-title position-relative text-uppercase mb-3"><span
                            className="bg-secondary pr-3">Filter by price</span></h5>
                        <div className="bg-light p-4 mb-30">
                            <form>

                                {maxPrice && (
                                    <CatalogPriceFilter onPriceChange={handlePriceChange}
                                                        firstValueMaxPrice={maxPrice}/>
                                )}
                                <p>Выбранный диапазон: {priceRange.min} - {priceRange.max}</p>

                            </form>
                        </div>
                        {/*<!-- Price End -->*/}

                        {/*<!-- Category Start -->*/}
                        <h5 className="section-title position-relative text-uppercase mb-3"><span
                            className="bg-secondary pr-3">Filter by category</span></h5>
                        <div className="bg-light p-4 mb-30">
                            <form>
                                <div
                                    className="custom-control custom-radio d-flex align-items-center justify-content-between mb-3">
                                    <input type="radio"
                                           className="custom-control-input"
                                           id="all"
                                           name="category"
                                           value={""}
                                           checked={selectedCategory === ""}
                                           onChange={handleCategoryChange}/>
                                    <label className="custom-control-label"
                                           htmlFor="all">All Categories</label>
                                </div>

                                {categories.map((category) => (
                                    <div
                                        key={category}
                                        className="custom-control custom-radio d-flex align-items-center justify-content-between mb-3">
                                        <input type="radio"
                                               className="custom-control-input"
                                               id={category}
                                               name="category"
                                               value={category}
                                               checked={selectedCategory == category}
                                               onChange={handleCategoryChange }/>
                                        <label className="custom-control-label"
                                               htmlFor={category}>{category}</label>
                                    </div>
                                ))}


                            </form>
                        </div>
                        {/*<!-- Category End -->*/}


                    </div>
                    {/*<!-- Shop Sidebar End -->*/}


                    {/*<!-- Shop Product Start -->*/}
                    <div className="col-lg-9 col-md-8">
                        <div className="row pb-3">
                            <div className="col-12 pb-1">
                                <div className="d-flex align-items-center justify-content-between mb-4">
                                    <div>

                                    </div>
                                    <div className="ml-2">
                                        <div className="btn-group">
                                            <button className="btn btn-sm btn-light ml-1"
                                                    onClick={() => setIsAscending(!isAscending)}>
                                                <img src="/../../src/assets/img/sorting.png"
                                                     alt="sort"
                                                     style={{transform: isAscending ? "rotate(0deg)" : "rotate(180deg)"}}
                                                />
                                            </button>
                                            <button type="button" className="btn btn-sm btn-light dropdown-toggle"
                                                    data-toggle="dropdown">Sorting
                                            </button>
                                            <div className="dropdown-menu dropdown-menu-right">
                                                <button className="dropdown-item"
                                                        onClick={() => setSortBy("CreatedAt")}>Newness
                                                </button>
                                                <button className="dropdown-item"
                                                        onClick={() => setSortBy("Price")}>Price
                                                </button>
                                                <button className="dropdown-item"
                                                        onClick={() => setSortBy("Rating")}>Rating
                                                </button>
                                            </div>
                                        </div>
                                        <div className="btn-group ml-2">
                                            <button type="button" className="btn btn-sm btn-light dropdown-toggle"
                                                    data-toggle="dropdown">Showing
                                            </button>
                                            <div className="dropdown-menu dropdown-menu-right">
                                                <button className="dropdown-item"
                                                        value={9}
                                                        onClick={handleProductsOnPageChange}>9
                                                </button>
                                                <button className="dropdown-item"
                                                        value={18}
                                                        onClick={handleProductsOnPageChange}>18
                                                </button>
                                                <button className="dropdown-item"
                                                        value={27}
                                                        onClick={handleProductsOnPageChange}>27
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {products.map((product) => (
                                <OneProductTile key={product.id} product={product} size={"big"}/>
                            ))}


                        </div>
                    </div>
                    {/*<!-- Shop Product End -->*/}

                    <div className="col-12">
                        {maxPages && (
                            <NavPages maxPages={maxPages} currentPage={currentPage} setCurrentPage={setCurrentPage}/>
                        )}
                    </div>

                </div>
            </div>
            {/*<!-- Shop End -->*/
            }


        </div>
    )
}

export default ShopPage;
