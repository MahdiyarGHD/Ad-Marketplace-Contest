import { memo, useEffect } from "react";
import PageHeader, {
	PageHeaderButtons,
	PageHeaderTitle,
} from "../../components/PageHeader";
import Avatar from "../../components/Avatar";
import { useNavigate, useParams } from "react-router-dom";
import { requestAPI } from "../../utils/api";
import useChannelStore from "../../stores/useChannelStore";
import { useShallow } from "zustand/shallow";
import { backButton } from "@tma.js/sdk-react";
import { buildClassName, invokeHapticFeedbackImpact } from "../../utils/common";
import "../Statistics.scss";

function ChannelProfile() {
	const { id } = useParams();

	const { chat_id, title, username } =
		useChannelStore(useShallow((state) => state.activeChannel)) || {};
	const { setActiveChannel } = useChannelStore();

	const navigate = useNavigate();

	const getChannelInfo = async () => {
		const response = await requestAPI(`/api/channels/${id}`, {}, "GET");

		setActiveChannel(response.value);
		console.log(response);
	};

	const onBackButton = () => {
		navigate("/");
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		if (!id) return;

		getChannelInfo();
	}, [id]);

	return (
		<div className="ChannelProfile Profile">
			<PageHeader>
				<PageHeaderTitle>Profile</PageHeaderTitle>
				<PageHeaderButtons />
			</PageHeader>

			<div className="Chat">
				<Avatar id={chat_id!} title={title ?? "C"} photo="" size={80} />
				<div className="info">
					<div className={buildClassName("title", !title && "Shimmer")}>
						{title}
					</div>
					<div className="subtitle">
						{username ? `@${username}` : "private channel"}
					</div>
				</div>
			</div>

			<div className="Statistics">
				<div className="Item">
					<div className="title">369</div>
					<div className="subtitle">Subscribers</div>
				</div>
				<div className="Item">
					<div className="title">369</div>
					<div className="subtitle">Subscribers</div>
				</div>
				<div className="Item">
					<div className="title">369</div>
					<div className="subtitle">Subscribers</div>
				</div>
			</div>
			<div className="Section">
				<div className="title">Pricing</div>
				<div className="StatItems">
					<div className="Item">
						<div className="flex-1">
							{/* <DollarSign /> */}
							<div className="title">100 TON</div>
							<div className="subtitle">Price</div>
						</div>
						<div>
							<div className="title">Per hour</div>
							<div className="subtitle">Price type</div>
						</div>
						<div>
							<div className="title">Post</div>
							<div className="subtitle">Ad format</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
}

export default memo(ChannelProfile);
