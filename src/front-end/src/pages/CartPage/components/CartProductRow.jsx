import {useEffect, useState} from "react";
import Path from "../../../endpoints.jsx";
import {useNavigate} from "react-router-dom";

const CartProductRow = ({item}) => {
    const navigate = useNavigate();
    const [quantity, setQuantity] = useState(item.quantity);

    const handleItemQuantityChange = (e) => {


        if (e.target.value > item.product.quantity || e.target.value < 0 || e.target.value > 99) {
            setQuantity(quantity);
            return;
        }
        setQuantity(e.target.value);
    }

    const handleMinusClick = () => {
        setQuantity(quantity - 1);
    }

    const handlePlusClick = () => {
        if (quantity + 1 > item.product.quantity || quantity + 1 > 99) {
            return;
        }
        setQuantity(quantity + 1);
    }


    useEffect(() => {
        if (quantity == item.quantity) {
            return
        }
        if (quantity > item.product.quantity) {
            setQuantity(item.product.quantity);
            return;
        }
        const updateCartQuantity = async () => {
            try {
                const token = localStorage.getItem("authToken");

                const response = await fetch(Path['cart'], {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                    body: JSON.stringify({
                        productId: item.product.id,
                        quantity: quantity,
                    }),
                });

                if (!response.ok) {
                    throw new Error("Failed to update cart quantity item");
                }

                console.log("Cart quantity item updated successfully");
                navigate(0);
            } catch (error) {
                console.error("Error updating cart quantity item:", error);
            }
        };

        updateCartQuantity();
    }, [quantity]);


    return (

        <tr>
            <td className="align-middle"><img src="/../../src/assets/img/product-1.jpg" alt=""
                                              style={{width: "50px"}}/> {item.product.name}
            </td>
            <td className="align-middle">${item.product.price}</td>
            <td className="align-middle">
                <div className="input-group quantity mx-auto" style={{width: "100px"}}>
                    <div className="input-group-btn">
                        <button className="btn btn-sm btn-primary btn-minus"
                        onClick={handleMinusClick}>
                            <i className="fa fa-minus"></i>
                        </button>
                    </div>
                    <input type="number"
                           className="form-control form-control-sm bg-secondary border-0 text-center"
                           value={quantity}
                           onChange={handleItemQuantityChange}

                    />
                    <div className="input-group-btn">
                        <button className="btn btn-sm btn-primary btn-plus"
                                onClick={handlePlusClick}>
                            <i className="fa fa-plus"></i>
                        </button>
                    </div>
                </div>
            </td>
            <td className="align-middle">${item.product.price * item.quantity}</td>
            <td className="align-middle">
                <button className="btn btn-sm btn-danger"><i className="fa fa-times"></i></button>
            </td>
        </tr>

    );
};

export default CartProductRow;