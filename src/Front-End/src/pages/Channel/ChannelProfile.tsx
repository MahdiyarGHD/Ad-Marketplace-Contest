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
import { invokeHapticFeedbackImpact } from "../../utils/common";

function ChannelProfile() {
	const { id } = useParams();

	const { title, username } =
		useChannelStore(useShallow((state) => state.activeChannel)) || {};

	const navigate = useNavigate();

	const getChannelInfo = async () => {
		const response = await requestAPI(`/api/channels/${id}`, {}, "GET");

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

			<div className="User">
				<Avatar id="user" title={title ?? ""} photo="" size={80} />
				<div className="info">
					<div className="title">{title}</div>
					<div className="subtitle">@{username}</div>
				</div>
			</div>
		</div>
	);
}

export default memo(ChannelProfile);
