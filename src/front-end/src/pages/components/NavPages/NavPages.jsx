import {useState} from "react";

const NavPages = ({maxPages, currentPage, setCurrentPage}) => {
        const pages = Array.from({ length: maxPages }, (_, index) => index + 1);


        return (

            <nav>
                <ul className="pagination justify-content-center">
                    <li className={`page-item ${currentPage === 1 ? 'disabled' : ""}`}>
                        <button className="page-link" onClick={() => setCurrentPage(1)}> {"<<"} </button>
                    </li>
                    <li className={`page-item ${currentPage === 1 ? 'disabled' : ""}`}>
                        <button className="page-link" onClick={() => setCurrentPage(currentPage-1)}> {"<"} </button>
                    </li>
                    {pages.map((page, index) => (
                        <li key={index} className={`page-item ${currentPage === page ? 'active' : ""}`}>
                            <button className="page-link"
                                    onClick={() =>setCurrentPage(page)}>{page}
                            </button>
                        </li>
                    ))}

                    <li className={`page-item ${currentPage === maxPages ? 'disabled' : ""}`}>
                        <button className="page-link" onClick={() => setCurrentPage(currentPage+1)}> {">"} </button>
                    </li>
                    <li className={`page-item ${currentPage === maxPages ? 'disabled' : ""}`}>
                        <button className="page-link" onClick={() => setCurrentPage(maxPages)}> {">>"} </button>
                    </li>
                </ul>
            </nav>

        )
    }
;
export default NavPages;