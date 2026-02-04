import { useEffect, useState } from "react";
import { backButton, mainButton, openTelegramLink } from "@tma.js/sdk-react";
import "./AddChannel.scss";
import { invokeHapticFeedbackImpact } from "../utils/common";
import { useNavigate, useParams } from "react-router-dom";
import PageHeader, { PageHeaderTitle } from "../components/PageHeader";
import RLottie from "../components/RLottie";
import useChannelStore from "../stores/useChannelStore";

function AddChannel() {
	const { status } = useParams();

	const [currentStatus, setCurrentStatus] = useState(status || "initial"); // initial, waiting, success

	const { unVerifiedChannels, verifyChannel, setDraftChannel } =
		useChannelStore();

	const navigate = useNavigate();

	const onBackButton = () => {
		navigate("/my-channels");
	};

	const onSelectChannel = () => {
		openTelegramLink(
			`https://t.me/${import.meta.env.VITE_BOT_USERNAME}?startchannel=true&admin=invite_users+promote_members`,
		);

		setCurrentStatus("waiting");

		invokeHapticFeedbackImpact("medium");

		mainButton.setText("Retry");
	};

	useEffect(() => {
		mainButton.setText("Select Channel");
		mainButton.onClick(onSelectChannel);
		mainButton.show();

		backButton.show();

		backButton.onClick(onBackButton);

		invokeHapticFeedbackImpact("medium");

		return () => {
			mainButton.hide();

			mainButton.offClick(onSelectChannel);

			backButton.hide();

			backButton.offClick(onBackButton);
		};
	}, []);

	useEffect(() => {
		if (currentStatus === "waiting") {
			const interval = setInterval(async () => {
				await verifyChannel();
			}, 5000);

			return () => clearInterval(interval);
		}
	}, [currentStatus]);

	useEffect(() => {
		if (unVerifiedChannels.length > 0) {
			setDraftChannel(unVerifiedChannels[0]);
			handleChannelVerified();
		}
	}, [unVerifiedChannels]);

	const handleChannelVerified = () => {
		console.log("Channel Verified", unVerifiedChannels);
		navigate("/set-channel-data");
		invokeHapticFeedbackImpact("medium");
		setTimeout(() => invokeHapticFeedbackImpact("soft"), 200);
		setTimeout(() => invokeHapticFeedbackImpact("soft"), 400);
	};

	const renderInstructions = () => {
		return (
			<div className="Placeholder">
				<div className="Emoji">
					<RLottie sticker="bubble" autoplay width={120} height={120} />
				</div>
				<h2 className="Title">Add Your Channel</h2>
				<div className="Instructions">
					<p className="InstructionText">
						1. Make sure you are an admin of the channel you want to add.
					</p>
					<p className="InstructionText">
						2. Select the channel from the list.
					</p>
					<p className="InstructionText">3. Add this bot to your channel.</p>
					<p className="InstructionText"> - Tap "Administrators".</p>
					<p className="InstructionText">
						{" "}
						- Tap "Add Admin" and search for our Bot.
					</p>
					<p className="InstructionText">
						{" "}
						- Grant the necessary permissions and save.
					</p>
					<p className="InstructionText">
						4. You're all set! Start managing your channel ads.
					</p>
				</div>
			</div>
		);
	};

	const renderWaiting = () => {
		return (
			<div className="Placeholder">
				<div className="Emoji">
					<RLottie sticker="waiting" autoplay loop width={120} height={120} />
				</div>
				<h2 className="Title">Waiting to add bot to your channel</h2>
				<div className="Instructions"></div>
			</div>
		);
	};

	const renderSuccess = () => {
		return (
			<div className="Placeholder">
				<div className="Emoji">
					<RLottie sticker="congrats" autoplay width={120} height={120} />
				</div>
				<h2 className="Title">Your Channel Successfully Added</h2>
				<div className="Instructions"></div>
			</div>
		);
	};

	return (
		<div className="AddChannel">
			<PageHeader>
				<PageHeaderTitle> </PageHeaderTitle>
			</PageHeader>

			{currentStatus === "waiting"
				? renderWaiting()
				: currentStatus === "success"
					? renderSuccess()
					: renderInstructions()}
		</div>
	);
}

export default AddChannel;
