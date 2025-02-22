import Config from "../config.jsx";

function MyFooter() {

    return (
        <div>
            {/*<!-- Footer Start -->*/}
            <div className="container-fluid bg-dark text-secondary mt-5 pt-5">
                <div className="row px-xl-5 pt-5">
                    <div className="col-lg-4 col-md-12 mb-5 pr-3 pr-xl-5">
                        <h5 className="text-secondary text-uppercase mb-4">Get In Touch</h5>
                        <p className="mb-4">Discover the latest tech innovations at unbeatable prices.
                          <br/>  Fast delivery, exceptional service, and all your favorite gadgets in one place!</p>

                        <p className="mb-2"><i className="fa fa-envelope text-primary mr-3"></i>{Config['email']}</p>
                        <p className="mb-0"><i className="fa fa-phone-alt text-primary mr-3"></i>{Config['phone']}</p>
                    </div>
                    <div className="col-lg-8 col-md-12">
                        <div className="row">
                            <div className="col-md-4 mb-5">
                                <h5 className="text-secondary text-uppercase mb-4">Quick Shop</h5>
                                <div className="d-flex flex-column justify-content-start">
                                    <a className="text-secondary mb-2" href="/"><i
                                        className="fa fa-angle-right mr-2"></i>Home</a>

                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div className="row border-top mx-xl-5 py-4" style={{borderColor: 'rgba(256, 256, 256, .1)'}}>
                    <div className="col-md-6 px-xl-0">
                        <p className="mb-md-0 text-center text-md-left text-secondary">
                            &copy; <a className="text-primary" href="#">Domain</a>. All Rights Reserved. Designed
                            by
                            <a className="text-primary" href="https://htmlcodex.com">HTML Codex</a>
                            <br/>Distributed By: <a href="https://themewagon.com" target="_blank">ThemeWagon</a>
                        </p>
                    </div>
                    <div className="col-md-6 px-xl-0 text-center text-md-right">
                        <img className="img-fluid" src="/assets/img/payments.png" alt=""/>
                    </div>
                </div>
            </div>
            {/*<!-- Footer End -->*/}




        </div>
)
}

export default MyFooter;
