import {Rating} from "react-simple-star-rating";

function OneProductTile({product, size}) {

    const columnClass = size === "small" ? "col-lg-3 col-md-4 col-sm-6 pb-1" : "col-lg-4 col-md-6 col-sm-6 pb-1";

    return (

        <div className={columnClass}>
            <div className="product-item bg-light mb-4">
                <a href={`/products/${product.id}`}>
                    <div className="product-img position-relative overflow-hidden" style={{aspectRatio: "1 / 1"}}>
                        <img className= "img-fluid w-100" src={product.imageUrl || "/../src/assets/img/no-image.jpeg"} alt="Product"
                             style={{objectFit: "contain"}} />
                        <div className="product-action" style={{opacity: "0.5"}}>
                        </div>
                    </div>
                </a>
                <div className="text-center py-4">
                    <a className="h6 text-decoration-none text-truncate" href={`/products/${product.id}`}>{product.name}</a>
                    <div className="d-flex align-items-center justify-content-center mt-2">
                        <h5>${product.price}</h5>
                    </div>

                        <div className="d-flex align-items-center justify-content-center mb-1"
                             style={{pointerEvents: "none", opacity: product.rating === 0 ? 0 : 1}}>
                            <Rating allowFraction={true} initialValue={product.rating} size={25}/>
                        </div>

                </div>
            </div>
        </div>


    )


}

export default OneProductTile;