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
import Deals from "./pages/Deals";
import SetChannelData from "./pages/SetChannelData";
import SelectCategory from "./pages/Channel/SelectCategory";
import SelectChannel from "./pages/Channel/SelectChannel";
import ChannelProfile from "./pages/Channel/ChannelProfile";
import AddCampaign from "./pages/AddCampaign";
import MyCampaigns from "./pages/MyCampaigns";
import CategoryPage from "./pages/CategoryPage";
import CampaignPage from "./pages/CampaignPage";

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

const router = createBrowserRouter(
	[
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
							path: "deals",
							element: <Deals />,
						},
						{
							path: "profile",
							element: <Profile />,
						},
						{
							path: "my-channels",
							element: <MyChannels />,
						},
					],
				},
				{
					element: <Main bottomBarVisible={false} />,
					children: [
						{
							path: "add-channel/:status?",
							element: <AddChannel />,
						},
						{
							path: "set-channel-data",
							element: <SetChannelData />,
						},
						{
							path: "select-channel",
							element: <SelectChannel />,
						},
						{
							path: "select-category/:set",
							element: <SelectCategory />,
						},
						{
							path: "/categories/:type?",
							element: <SelectCategory title="Browse Categories" />,
						},
						{
							path: "channel/:id",
							element: <ChannelProfile />,
						},
						{
							path: "category/:categoryId",
							element: <CategoryPage />,
						},
						{
							path: "add-campaign",
							element: <AddCampaign />,
						},
						{
							path: "add-campaign/success",
							element: <AddCampaign success />,
						},
						{
							path: "my-campaigns",
							element: <MyCampaigns />,
						},
						{
							path: "campaign/:id",
							element: <CampaignPage />,
						},
					],
				},
			],
		},
	],
	{
		basename: "/Ad-Marketplace-Contest",
	},
);

export default function Routes() {
	return <RouterProvider router={router} />;
}
