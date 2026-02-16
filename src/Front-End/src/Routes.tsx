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
import SelectMyChannel from "./pages/Channel/SelectMyChannel";
import Applications from "./pages/Applications";
import SelectMyCampaign from "./pages/SelectMyCampaign";

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
							path: "deals/:id?",
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
							path: "select-my-channel/:set",
							element: <SelectMyChannel />,
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
							path: "channel/:id/invite?",
							element: <ChannelProfile />,
						},
						{
							path: "channel/:id/applications",
							element: <Applications type="channel" />,
						},
						{
							path: "channel/:id/invitations",
							element: <Applications type="channel" invite />,
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
							path: "edit-campaign/:id",
							element: <AddCampaign />,
						},
						{
							path: "my-campaigns",
							element: <MyCampaigns />,
						},
						{
							path: "campaign/:id/apply?",
							element: <CampaignPage />,
						},
						{
							path: "campaign/:id/applications",
							element: <Applications type="campaign" />,
						},
						{
							path: "campaign/:id/invitations",
							element: <Applications type="campaign" invite />,
						},
						{
							path: "select-my-campaign/:set",
							element: <SelectMyCampaign />,
						},
						{
							path: "my-channel-applications",
							element: <Applications type="advertiser" />,
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
