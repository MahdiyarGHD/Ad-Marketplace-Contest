import { retrieveRawInitData, useLaunchParams } from "@tma.js/sdk-react";
import { HomeIcon, MessageCircle } from "lucide-react";
import { memo, useEffect } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { buildClassName } from "../utils/common";

function BottomBar() {
	const launchParams = useLaunchParams();
	const rawInitData = retrieveRawInitData();

	const location = useLocation();
	const navigate = useNavigate();

	const firstName = launchParams.tgWebAppData?.user?.first_name;

	useEffect(() => {
		console.log(rawInitData);
	}, []);

	return (
		<div className="BottomBar">
			<div
				className={buildClassName(
					"Item",
					location.pathname === "/" && "active",
				)}
				onClick={() => navigate("/")}
			>
				<div className="meta">
					<HomeIcon />
				</div>
				<div className="title">Home</div>
			</div>
			<div
				className={buildClassName(
					"Item",
					location.pathname === "/my-channels" && "active",
				)}
				onClick={() => navigate("/my-channels")}
			>
				<div className="meta">
					<MessageCircle />
				</div>
				<div className="title">My Channels</div>
			</div>
			<div className="Item">
				<div className="meta">
					<div className="Avatar">
						<div className="title">{firstName?.charAt(0).toUpperCase()}</div>
					</div>
				</div>
				<div className="title">{firstName}</div>
			</div>
		</div>
	);
}

export default memo(BottomBar);
