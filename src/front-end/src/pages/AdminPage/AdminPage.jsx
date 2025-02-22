import "./AdminPage.css"
import {useEffect, useState} from "react";
import Path from "../../endpoints.jsx";
import {useCategories} from "../../functions/CategoryContext.jsx";
import {useLocation} from "react-router-dom";
import NavPages from "../components/NavPages/NavPages.jsx";
import OneProductRow from "./components/OneProductRow.jsx";
import {jwtDecode} from "jwt-decode";

function AdminPage() {

    const [products, setProducts] = useState([]);
    const [maxPages, setMaxPages] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const [productsOnPage, setProductsOnPage] = useState(5);
    const [selectedCategory, setSelectedCategory] = useState("");
    const [isAddingNew, setIsAddingNew] = useState(false);
    const [search, setSearch] = useState("");
    const [tempSearch, setTempSearch] = useState("");
    const categories = useCategories();


    const location = useLocation();


    useEffect(() => {
        if (location.state?.selectedCategory) {
            setSelectedCategory(location.state.selectedCategory);
        }
    }, [location.state]);

    useEffect(() => {
        fetch(`${Path['product']}` +
            `?quantity=${productsOnPage}` +
            `&page=${currentPage}` +
            `&sortBy=CreatedAt`+
            `&isAscending=false`+
            `${selectedCategory ? `&category=${selectedCategory}` : ''}`+
            `${search ? `&name=${search}` : ''}`)
            .then((response) => response.json())
            .then((data) => {
                setProducts(data.products);
                setMaxPages(data.pages);
            })
            .catch((error) => console.error("Error fetching data:", error));

        //console.log(products);

    }, [productsOnPage, currentPage, selectedCategory, search]);

    const handleCategoryChange = (e) => {
        setCurrentPage(1);
        setSelectedCategory(e.target.value); // Обновляем состояние при выборе новой категории
    };


    const handleProductsOnPageChange = (e) => {
        setCurrentPage(1);
        setProductsOnPage(e.target.value); // Обновляем состояние выбранного значения
    };

    const handleSearchChange = (e) => {
        setCurrentPage(1);
        setTempSearch(e.target.value);  // Обновляем состояние при вводе текста
    };

    const handleSearchSubmit = (e) => {
        e.preventDefault();
        setSearch(tempSearch)  // Перенаправляем на /products с query параметром
    };


const token = localStorage.getItem("authToken");
if(token) {
    const decodedToken = jwtDecode(token);

    if(decodedToken.IsAdmin == "True") {

        return (

            <div id="admin-page">
                <div className="table-responsive">
                    <div className="table-wrapper">
                        <div className="table-title">
                            <div className="row">
                                <div className="col-sm-4">
                                    <h2 style={{color: "white"}}><b>Admin panel</b></h2>
                                </div>
                                <div className="col-sm-8">
                                    <button type="button"
                                            className="btn btn-info add-new"
                                            onClick={() => setIsAddingNew(true)}
                                    >
                                        <i
                                            className="fa fa-plus"></i> Add New
                                    </button>

                                </div>
                            </div>
                        </div>
                        <div className="table-filter">
                            <div className="row">
                                <div className="col-sm-3">
                                    <div className="show-entries">
                                        <span>Show</span>
                                        <select className="form-control"
                                        value={productsOnPage}
                                        onChange={handleProductsOnPageChange}>
                                            <option value={5}>5</option>
                                            <option value={10}>10</option>
                                            <option value={15}>15</option>
                                            <option value={20}>20</option>
                                        </select>
                                        <span>products</span>
                                    </div>
                                </div>
                                <div className="col-sm-9">

                                    <form onSubmit={handleSearchSubmit}>
                                    <button type="submit" className="btn btn-primary"><i className="fa fa-search"></i>
                                    </button>
                                    <div className="filter-group">
                                        <label>Name</label>
                                        <input type="text"
                                               value={tempSearch}
                                               onChange={handleSearchChange}
                                               className="form-control"/>
                                    </div>
                                    </form>

                                    <div className="filter-group">
                                        <label>Category</label>
                                        <select className="form-control"
                                                value={selectedCategory}
                                                onChange={handleCategoryChange}
                                        >
                                            <option value={""}>All</option>
                                            {categories.map((category) => (
                                                <option key={category} value={category}>{category}</option>
                                            ))}
                                        </select>
                                    </div>
                                    <span className="filter-icon"><i className="fa fa-filter"></i></span>
                                </div>


                            </div>
                        </div>


                        <table className="table table-bordered">
                            <thead>
                            <tr>
                                <th>Id</th>
                                <th>Photo</th>
                                <th>Product Name</th>
                                <th>Description</th>
                                <th>Category</th>
                                <th>Price in $</th>
                                <th>Quantity</th>
                                <th>Rating</th>
                                <th>Created at</th>
                                <th>Updated at</th>
                                <th>Actions</th>
                            </tr>
                            </thead>
                            <tbody>

                            {isAddingNew && (
                                <OneProductRow product={{}} isNew={true}/>
                            )}

                            {products.map((product) => (

                                <OneProductRow key={product.id} product={product} isNew={false}/>


                                // <tr>
                                //     <td> <div className={"id"}>{product.id} </div></td>
                                //     <td> <div className={"photo"}><img src="#" className="photo" alt="Photo"/></div> </td>
                                //     <td> <div className={"name"}>{product.name}</div> </td>
                                //     <td> <div className={"description"}>{product.description}</div> </td>
                                //     <td> <div className={"category"}>{product.category}</div> </td>
                                //     <td> <div className={"price"}>{product.price}</div> </td>
                                //     <td> <div className={"quantity"}>{product.quantity}</div> </td>
                                //     <td> <div className={"rating"}>{product.rating}/5</div> </td>
                                //     <td> <div className={"createdAt"}>{product.createdAt}</div> </td>
                                //     <td> <div className={"updatedAt"}>{product.updatedAt}</div> </td>
                                //     <td className={"actions"}>
                                //         <a className="add" title="Add" data-toggle="tooltip"><i
                                //             className="material-icons">&#xE03B;</i></a>
                                //         <a className="edit" title="Edit" data-toggle="tooltip"><i
                                //             className="material-icons">&#xE254;</i></a>
                                //         <a className="delete"  data-toggle="tooltip"><i
                                //             className="material-icons">&#xE872;</i></a>
                                //     </td>
                                // </tr>
                            ))}
                            </tbody>
                        </table>

                        {/*<table className="table table-striped table-hover">*/}
                        {/*    <thead>*/}
                        {/*    <tr>*/}
                        {/*        <th>Id</th>*/}
                        {/*        <th>Photo</th>*/}
                        {/*        <th>Product Name</th>*/}
                        {/*        <th>Description</th>*/}
                        {/*        <th>Category</th>*/}
                        {/*        <th>Price</th>*/}
                        {/*        <th>Quantity</th>*/}
                        {/*        <th>Rating</th>*/}
                        {/*        <th>Created at</th>*/}
                        {/*        <th>Updated at</th>*/}
                        {/*        <th>Update</th>*/}
                        {/*        <th>Delete</th>*/}

                        {/*    </tr>*/}
                        {/*    </thead>*/}
                        {/*    <tbody>*/}

                        {/*    {products.map((product) => (*/}
                        {/*        <tr>*/}
                        {/*            <td>{product.id}</td>*/}
                        {/*            <td><img src="#" className="avatar" alt="Avatar"/></td>*/}
                        {/*            <td>{product.name}</td>*/}
                        {/*            <td>{product.description}</td>*/}
                        {/*            <td>{product.category}</td>*/}
                        {/*            <td>${product.price}</td>*/}
                        {/*            <td>${product.quantity}</td>*/}
                        {/*            <td>{product.rating}/5</td>*/}
                        {/*            <td>{product.createdAt}</td>*/}
                        {/*            <td>{product.updatedAt}</td>*/}
                        {/*            <td><a href="#" className="view" title="View Details" data-toggle="tooltip"><i*/}
                        {/*                className="material-icons">&#xE5C8;</i></a></td>*/}
                        {/*            <td><a href="#" className="view" title="View Details" data-toggle="tooltip"><i*/}
                        {/*                className="material-icons">&#xE5C8;</i></a></td>*/}
                        {/*        </tr>*/}
                        {/*    ))}*/}


                        {/*    </tbody>*/}
                        {/*</table>*/}
                        <div className="clearfix">
                            {maxPages && (
                                <NavPages maxPages={maxPages} currentPage={currentPage} setCurrentPage={setCurrentPage}/>
                            )}
                        </div>
                    </div>
                </div>
            </div>
        )
    }


    }

    return (
        <div>
            ACCESS DENIED
        </div>
    )

}




export default AdminPage;