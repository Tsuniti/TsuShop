import {useNavigate, useParams} from "react-router-dom";
import {useEffect, useState} from "react";
import Path from "../../endpoints.jsx";
import {Rating} from "react-simple-star-rating";
import {jwtDecode} from "jwt-decode";

function ProductPage() {


    const navigate = useNavigate();
    const [usersReview, setUsersReview] = useState(null);
    const [userId, setUserId] = useState("");
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [isUserAdmin, setIsUserAdmin] = useState(false);
    const [reviewRating, setReviewRating] = useState(0);
    const [reviewText, setReviewText] = useState("");
    const [selectedQuantity, setselectedQuantity] = useState(1);


    const [product, setProduct] = useState();

    const {productId} = useParams();


    useEffect(() => {
        fetch(`${Path['product']}/${productId}`)
            .then((response) => response.json())
            .then((data) => {
                setProduct(data);
            })
            .catch((error) => console.error("Error fetching data:", error));
    }, [productId]);


    useEffect(() => {
        const token = localStorage.getItem("authToken");
        //console.log(token);

        if (token) {
            try {
                const decodedToken = jwtDecode(token);
                const expirationTime = decodedToken.exp * 1000; // Преобразуем в миллисекунды
                if (Date.now() < expirationTime) {
                    setIsAuthenticated(true);  // Токен действителен
                    setIsUserAdmin(decodedToken.IsAdmin);
                    setUserId(decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"]);

                } else {
                    console.log('expired token: ' + token);
                    localStorage.removeItem("authToken");  // Токен просрочен, удаляем его
                    setIsAuthenticated(false);
                    setIsUserAdmin(false);
                }
            } catch (error) {
                console.log('token error: ' + error);
                localStorage.removeItem("authToken");  // Ошибка в декодировании токена, удаляем его
                setIsAuthenticated(false);
                setIsUserAdmin(false);
            }
        }
    },);


    useEffect(() => {
        if (product && product.reviews) {

            const review = product.reviews.find((review) => review.userId === userId);

            if (review) {

                setUsersReview(review);
                setReviewRating(review.rating);
                setReviewText(review.text);
            }
        }
    }, [product, userId]);


    const handleSubmit = async (event) => {
        event.preventDefault();

        if (reviewRating < 1 || reviewRating > 5) {
            alert("Rating must be 1 to 5 stars.");
            return;
        }

        const token = localStorage.getItem("authToken")

        const reviewData = {
            ...(usersReview ? {reviewId: usersReview.id} : {productId: productId}),
            rating: reviewRating,  // Use the state for rating
            text: reviewText, // Use the state for review text
        };

        try {
            const response = await fetch(Path['review'], {
                method: `${usersReview ? "PUT" : "POST"}`,
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify(reviewData),
            });

            if (response.ok) {
                navigate(0);
            } else {
                alert('There was an issue with submitting your review');
            }
        } catch (error) {
            alert('Error: ' + error.message);
        }
    }

    const handleItemQuantityChange = (e) => {


        if (e.target.value > product.quantity || e.target.value < 1 || e.target.value > 99) {
            setselectedQuantity(selectedQuantity);
            return;
        }
        setselectedQuantity(e.target.value);
    }

    const handlePlusClick = () => {
        if (setselectedQuantity() + 1 > product.quantity || selectedQuantity + 1 > 99) {
            return;
        }
        setselectedQuantity(selectedQuantity + 1);
    }

    const handleMinusClick = () => {
        if (selectedQuantity > 1) {
            setselectedQuantity(selectedQuantity - 1);
        }
    }

    const handleAddToCartClick = async () => {


        if (!isAuthenticated) {
            navigate(Path['login']);
        }

        try {
            const token = localStorage.getItem("authToken");

            const response = await fetch(Path['cart'], {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    Authorization: `Bearer ${token}`,
                },
                body: JSON.stringify({
                    productId: product.id,
                    quantity: selectedQuantity > 0 ? selectedQuantity : 1,
                }),
            });

            if (!response.ok) {
                throw new Error("Failed to update cart quantity item");
            }

            console.log("Cart quantity item updated successfully");
            navigate('/cart', {state: {selectedQuantity}});
        } catch (error) {
            console.error("Error updating cart quantity item:", error);
        }
    };


    const deleteReviewHandleClick = async (reviewId) => {

        try {
            const token = localStorage.getItem("authToken");

            // Проверяем, есть ли токен
            if (!token) {
                console.error("No authentication token found.");
                return;
            }

            const response = await fetch(Path['review'], {
                method: "DELETE",
                headers: {
                    "Authorization": `Bearer ${token}`,
                    "Content-Type": "application/json",  // Устанавливаем Content-Type для JSON
                },
                body: JSON.stringify({
                    reviewId: reviewId,
                }),
            });

            if (!response.ok) {
                throw new Error("Failed to delete review");
            }
            navigate(0);
            console.log("Review deleted successfully");
        } catch (error) {
            console.error("Error deleting review:", error);
        }


    }


    if (!product) {
        return <div>Loading...</div>
    }


    return (

        <div className="container-fluid pb-5">
            <div className="row px-xl-5">
                <div className="col-lg-5 mb-30">
                    <div id="product-carousel" className="carousel slide" data-ride="carousel">
                        <div className="carousel-inner bg-light">
                            <div className="carousel-item active">
                                <img className="w-100 h-100"
                                     src={product.imageUrl || "/../src/assets/img/no-image.jpeg"} alt="Image"
                                />
                            </div>
                        </div>
                    </div>
                </div>

                <div className="col-lg-7 h-auto mb-30">
                    <div className="h-100 bg-light p-30">
                        <h3>{product.name}</h3>
                        <div className="d-flex mb-3">
                            <div className="d-flex align-items-center justify-content-center mb-1"
                                 style={{pointerEvents: "none", opacity: product.rating === 0 ? 0 : 1}}>
                                <Rating allowFraction={true} initialValue={product.rating} size={25}/>
                            </div>
                            <small className="pt-1">({product.reviews?.length || 0} Reviews)</small>
                        </div>
                        <h3 className="font-weight-semi-bold mb-4">${product.price}</h3>
                        <p className="mb-4">{product.description}</p>
                        <div className="d-flex align-items-center mb-4 pt-2">
                            <div className="input-group quantity mr-3" style={{width: "130px"}}>
                                <div className="input-group-btn">
                                    <button className="btn btn-primary btn-minus"
                                            onClick={handleMinusClick}>
                                        <i className="fa fa-minus"></i>
                                    </button>
                                </div>
                                <input type="text" className="form-control bg-secondary border-0 text-center"
                                       value={selectedQuantity}
                                       onChange={handleItemQuantityChange}/>
                                <div className="input-group-btn">
                                    <button className="btn btn-primary btn-plus"
                                            onClick={handlePlusClick}>
                                        <i className="fa fa-plus"></i>
                                    </button>
                                </div>
                            </div>
                            <button className="btn btn-primary px-3" onClick={handleAddToCartClick}><i
                                className="fa fa-shopping-cart mr-1"
                            ></i> Add To
                                Cart
                            </button>
                        </div>
                    </div>
                </div>
            </div>
            <div className="row px-xl-5">
                <div className="col">
                    <div className="bg-light p-30">
                        <div className="tab-content">
                            <div className="row">
                                <div className="col-md-6">
                                    <h4 className="mb-4">{product.reviews?.length || 0} review for {product.name}</h4>

                                    {product.reviews.map((review) => (
                                        <div key={review.id} className="mb-5">
                                            <div className="media-body">
                                                <div className="d-flex justify-content-between">
                                                    <h6>{review.user.username}
                                                        <small> - <i>{new Date(review.createdAt).toLocaleString()}</i></small>
                                                    </h6>
                                                    <div>
                                                    <div
                                                        className="d-flex align-items-center justify-content-center mb-2 mr-4"
                                                        style={{
                                                            pointerEvents: "none",
                                                            opacity: product.rating === 0 ? 0 : 1
                                                        }}>
                                                        <Rating initialValue={review.rating} size={20}/>
                                                    </div>
                                                    {(isUserAdmin == "True"
                                                            || review.userId == userId)
                                                        &&
                                                        (<a className="text-danger ml-5"
                                                            href="#"
                                                            onClick={() => deleteReviewHandleClick(review.id)}>
                                                            delete
                                                        </a>)}

                                                </div>
                                                </div>


                                                <p style={{overflowWrap: 'break-word'}}>{review.text}</p>

                                                {new Date(review.updatedAt).getTime() > new Date(review.createdAt).getTime() + 10000 && (
                                                    <small className="float-right mr-4">
                                                        <i>updated - {new Date(review.updatedAt).toLocaleString()}</i>
                                                    </small>
                                                )}
                                            </div>
                                        </div>
                                    ))}


                                </div>
                                <div className="col-md-6">


                                    {isAuthenticated ? (
                                        <div>
                                            <h4 className="mb-4">{usersReview ? "Edit your review" : "Leave a review"}</h4>

                                            <form onSubmit={handleSubmit}>
                                                <div className="form-group">
                                                    <div className="d-flex my-3">
                                                        <label className="mb-0 mr-2">Your Rating:</label>
                                                        <Rating
                                                            initialValue={reviewRating}
                                                            size={23}
                                                            onClick={(value) => setReviewRating(value)}
                                                        />
                                                    </div>
                                                </div>

                                                <div className="form-group">
                                                    <label htmlFor="message">Your Review</label>
                                                    <textarea
                                                        id="message"
                                                        cols="30"
                                                        rows="5"
                                                        className="form-control"
                                                        value={reviewText}
                                                        onChange={(e) => setReviewText(e.target.value)}
                                                    ></textarea>
                                                </div>

                                                <div className="form-group mb-0">
                                                    <input
                                                        type="submit"
                                                        value={usersReview ? "Save Your Review" : "Leave Your Review"}
                                                        className="btn btn-primary px-3"
                                                    />
                                                </div>
                                            </form>
                                        </div>
                                    ) : (
                                        <div className="alert text-center" role="alert">
                                            <h4>To leave a review, please login.</h4>
                                        </div>
                                    )}

                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


    )
}

export default ProductPage;