import { Suspense } from "react";
import {
	createBrowserRouter,
	Outlet,
	RouterProvider,
	useLocation,
} from "react-router-dom";
import Main from "./components/Main";
import Home from "./pages/Home";
import MyChannels from "./pages/MyChannels";
import AddChannel from "./pages/AddChannel";
import Profile from "./pages/Profile";

const Loading = () => {
	return <div className="LoadingBar">Loading...</div>;
};

const RootLayout = () => {
	const location = useLocation();
	return (
		<div key={location.pathname} style={{ height: "100%" }}>
			<Suspense fallback={<Loading />}>
				<Outlet />
			</Suspense>
		</div>
	);
};

const router = createBrowserRouter([
	{
		path: "/",
		element: <RootLayout />,
		children: [
			{
				element: <Main />,
				children: [
					{
						index: true,
						element: <Home />,
					},
					{
						path: "my-channels",
						element: <MyChannels />,
					},
					{
						path: "profile",
						element: <Profile />,
					},
				],
			},
			{
				path: "add-channel",
				element: <AddChannel />,
			},
		],
	},
]);

export default function Routes() {
	return <RouterProvider router={router} />;
}
