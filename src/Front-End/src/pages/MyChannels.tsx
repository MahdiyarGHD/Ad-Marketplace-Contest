import { memo, useEffect } from "react";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import Avatar from "../components/Avatar";
import { PlusIcon } from "lucide-react";
import "./MyChannels.scss";
import { useNavigate } from "react-router-dom";
import Transition from "../components/Transition";
import { backButton } from "@tma.js/sdk-react";
import { invokeHapticFeedbackImpact } from "../utils/common";
import useChannelStore from "../stores/useChannelStore";

const ChannelStatus: { [key: number]: string } = {
	0: "Pending",
	1: "Pending",
	2: "Active",
	3: "Inactive",
};

function MyChannels() {
	const { myChannels, getMyChannels } = useChannelStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/profile");
	};

	useEffect(() => {
		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		getMyChannels();

		return () => {
			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	return (
		<div className="MyChannels">
			<PageHeader>
				<PageHeaderTitle>My Channels</PageHeaderTitle>
			</PageHeader>

			<div className="ChatList">
				<div className="Item primary" onClick={() => navigate("/add-channel")}>
					<div className="icon">
						<PlusIcon />
					</div>
					<div className="title">Add Your Channel</div>
				</div>
				<Transition
					state
					eachElement
					eachElementDelay={40}
					key={myChannels?.length}
				>
					{myChannels.map((channel) => (
						<div
							key={channel.chat_id}
							className="ChatItem"
							onClick={() => navigate("/set-channel-data")}
						>
							<Avatar id={channel.chat_id} title={channel.title} photo="" />
							<div className="body">
								<div className="title">{channel.title}</div>
								<div className="subtitle">{channel.chat_id}</div>
							</div>
							<div className="meta">{ChannelStatus[channel.status]}</div>
						</div>
					))}
				</Transition>
			</div>

			<div className="NoChannel"></div>
		</div>
	);
}

export default memo(MyChannels);
