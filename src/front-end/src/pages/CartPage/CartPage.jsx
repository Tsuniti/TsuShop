import {useEffect, useState} from "react";
import Path from "../../endpoints.jsx";
import {jwtDecode} from "jwt-decode";
import CartProductRow from "./components/CartProductRow.jsx";

function CartPage() {


    const [cart, setCart] = useState([]);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [token, setToken] = useState(null);
    const [totalPriceMap, setTotalPriceMap] = useState({});
    const [totalCartPrice, setTotalCartPrice] = useState({});


    useEffect(() => {
        const token = localStorage.getItem("authToken");
        //console.log(token);

        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                const expirationTime = decodedToken.exp * 1000; // Преобразуем в миллисекунды
                if (Date.now() < expirationTime) {
                    setToken(token);

                } else {
                    console.log('expired token: ' + token);
                    localStorage.removeItem("authToken");  // Токен просрочен, удаляем его
                    setToken(null);
                }
            } catch (error) {
                console.log('token error: ' + error);
                localStorage.removeItem("authToken");  // Ошибка в декодировании токена, удаляем его
                setToken(null);
            }
        }
    }, []);


    useEffect(() => {
        if (!token) return;

        const fetchCart = async () => {
            try {
                const response = await fetch(Path['cart'], {
                    method: 'GET',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (!response.ok) {
                    console.error('Response error:', response);
                    alert('There was an issue with fetching your cart');
                    return;
                }

                const data = await response.json(); // Используем response.json() вместо JSON.parse(response.body)
                setCart(data);
            } catch (error) {
                console.error('Fetch error:', error);
                alert('Error: ' + error.message);
            }
        };

        fetchCart();
    }, [token]);



    return (
        <div className="container-fluid">
            <div className="row px-xl-5">
                <div className="col-lg-8 table-responsive mb-5">
                    <table className="table table-light table-borderless table-hover text-center mb-0">
                        <thead className="thead-dark">
                        <tr>
                            <th>Products</th>
                            <th>Price</th>
                            <th>Quantity</th>
                            <th>Total</th>
                            <th>Remove</th>
                        </tr>
                        </thead>
                        <tbody className="align-middle">



                        {cart.map((item) => (

                            <CartProductRow item={item} key={item.id} />

                        ))}





                        </tbody>
                    </table>
                </div>
                <div className="col-lg-4">

                    <h5 className="section-title position-relative text-uppercase mb-3"><span
                        className="bg-secondary pr-3">Cart Summary</span></h5>
                    <div className="bg-light p-30 mb-5">

                        <div className="pt-2">
                            <div className="d-flex justify-content-between mt-2">
                                <h5>Total</h5>
                                <h5>${cart.reduce((sum, item) => sum + (item.product.price * item.quantity), 0)}</h5>
                            </div>
                            <button className="btn btn-block btn-primary font-weight-bold my-3 py-3">Proceed To
                                Checkout
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    )
}

export default CartPage;