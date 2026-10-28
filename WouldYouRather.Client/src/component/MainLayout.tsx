import { Outlet } from "react-router-dom";
import Navbar from "./NavBar";

export default function MainLayout() {
	return (
		<>
			<Navbar />
			<Outlet />
		</>
	);
}
