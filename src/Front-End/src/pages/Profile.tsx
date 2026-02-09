import { memo } from "react";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../components/PageHeader";
import Avatar from "../components/Avatar";
import "./Profile.scss";
import { useLaunchParams } from "@tma.js/sdk-react";
import {
	ChevronRightIcon,
	DollarSignIcon,
	MegaphoneIcon,
	MessageCircleIcon,
} from "lucide-react";
import { useNavigate } from "react-router-dom";

function Profile() {
	const launchParams = useLaunchParams();
	const navigate = useNavigate();

	const { id, first_name, username } = launchParams.tgWebAppData?.user || {};

	return (
		<div className="Profile">
			<PageHeader>
				<PageHeaderTitle>Profile</PageHeaderTitle>
				<PageHeaderButtons />
			</PageHeader>

			<div className="User">
				<Avatar id={id!} title={first_name ?? ""} photo="" size={80} />
				<div className="info">
					<div className="title">{first_name}</div>
					<div className="subtitle">@{username}</div>
				</div>
			</div>

			<div className="Section">
				<div className="Items">
					<div className="Item">
						<div className="icon">
							<DollarSignIcon />
						</div>
						<div className="body">
							<div className="title">Balance</div>
						</div>
						<div className="meta">0.00 TON</div>
					</div>
				</div>
			</div>

			<div className="Section">
				<div className="Items">
					<div className="Item" onClick={() => navigate("/my-channels")}>
						<div className="icon">
							<MessageCircleIcon />
						</div>
						<div className="body">
							<div className="title">My Channels</div>
						</div>
						<div className="meta">
							<ChevronRightIcon />
						</div>
					</div>
					<div className="Item" onClick={() => navigate("/my-campaigns")}>
						<div className="icon">
							<MegaphoneIcon />
						</div>
						<div className="body">
							<div className="title">My Campaigns</div>
						</div>
						<div className="meta">
							<ChevronRightIcon />
						</div>
					</div>
				</div>
			</div>
		</div>
	);
}

export default memo(Profile);
