import { ChevronRight } from "lucide-react";
import PageHeader, { PageHeaderTitle } from "../../components/PageHeader";
import Transition from "../../components/Transition";
import { memo, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { invokeHapticFeedbackImpact } from "../../utils/common";
import { backButton } from "@tma.js/sdk-react";
import useChannelStore from "../../stores/useChannelStore";
import Avatar from "../../components/Avatar";

function SelectChannel() {
	const { unVerifiedChannels, setDraftChannel } = useChannelStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/set-channel-data");
	};

	const onSelectChannel = (channel: any) => {
		setDraftChannel(channel);
		navigate("/set-channel-data");
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

	return (
		<div className="SelectChannel scrollable">
			<PageHeader>
				<PageHeaderTitle>Select Channel</PageHeaderTitle>
			</PageHeader>

			<div className="ChannelList Items">
				<Transition state eachElement eachElementDelay={20}>
					{unVerifiedChannels.map((channel) => (
						<div
							key={channel.chat_id}
							className="ChatItem"
							onClick={() => onSelectChannel(channel)}
						>
							<Avatar id={channel.chat_id} title={channel.title} photo="" />
							<div className="body">
								<div className="title">{channel.title}</div>
								<div className="subtitle">subtitle</div>
							</div>
							<div className="meta">
								<ChevronRight />
							</div>
						</div>
					))}
				</Transition>
			</div>
			{unVerifiedChannels.length === 0 && (
				<div className="NothingPlaceholder">
					No unverified channels available.
					<br /> Please add a channel first.
				</div>
			)}
		</div>
	);
}

export default memo(SelectChannel);
