import { retrieveRawInitData, useLaunchParams } from "@tma.js/sdk-react";
import {
	HandshakeIcon,
	HomeIcon,
	MessageCircle,
	StoreIcon,
} from "lucide-react";
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
					<StoreIcon />
				</div>
				<div className="title">Market</div>
			</div>
			<div
				className={buildClassName(
					"Item",
					location.pathname === "/my-channels" && "active",
				)}
				onClick={() => navigate("/my-channels")}
			>
				<div className="meta">
					<HandshakeIcon />
				</div>
				<div className="title">Deals</div>
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
